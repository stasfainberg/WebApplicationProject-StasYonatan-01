﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TachzukanitBE.Data;
using TachzukanitBE.Models;
using TachzukanitBE.Services;
using TachzukanitBE.ViewModels;

namespace TachzukanitBE.Controllers
{
    public class MalfunctionsController : Controller
    {
        private readonly TachzukanitDbContext _context;
        private readonly IHostingEnvironment he;

        public MalfunctionsController(TachzukanitDbContext context, IHostingEnvironment e)
        {
            _context = context;
            he = e;
        }

        // POST: Get malfunctions parameters from the user and search in server
        //       it shows the malfunctions details and also user name, appartment password
        public async Task<IActionResult> ShowMalfExtraDetails(DateTime createDate, Status status,
                                                              string address, string userName)
        {
            if ((createDate == null) && (String.IsNullOrEmpty(status.ToString())) &&
                  String.IsNullOrEmpty(address) && String.IsNullOrEmpty(userName))
            {
                return View(await _context.Malfunction.ToListAsync());
            }

            return (await SearchMalfExtraDetails(createDate, status, address, userName));
        }

        private async Task<IActionResult> SearchMalfExtraDetails(DateTime createDate, Status status,
                                                              string address, string userName)
        {
            Status enumStatus = (Status)status;

            var q = from malfunction in _context.Malfunction
                    join apartments in _context.Apartment on malfunction.CurrentApartment equals apartments
                    join users in _context.User on malfunction.RequestedBy equals users
                    where malfunction.CreationDate >= createDate &&
                          malfunction.Status.Equals(enumStatus) &&
                          apartments.Address.Equals(address) &&
                          malfunction.RequestedBy.Email.Contains(userName)
                    select new ExtraDetailsMalfunctionsVM()
                    {
                        Title = malfunction.Title,
                        Status = malfunction.Status.ToString(),
                        Content = malfunction.Content,
                        CreationDate = malfunction.CreationDate,
                        ModifiedDate = malfunction.ModifiedDate,
                        AppartmentAddress = apartments.Address,
                        UserName = users.FullName
                    };

             return View(await q.ToListAsync());
    }

        // GET: Malfunctions
        [Authorize(Roles = "Admin,Janitor,Guide,SocialWorker")]
        public async Task<IActionResult> Index(String searchString)
        {
            var databaseContext = _context.Malfunction.Include(p => p.CurrentApartment).Include(ps => ps.RequestedBy);

            if (String.IsNullOrEmpty(searchString))
            {
                return View(await databaseContext.ToListAsync());
            }

            return View(await databaseContext.Where(m => m.Content.Contains(searchString)).ToListAsync());
        }

        // GET: Malfunctions/Details/5
        [Authorize(Roles = "Admin,Janitor,Guide,SocialWorker")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFoundPage");
            }

            var malfunction = await _context.Malfunction
                .FirstOrDefaultAsync(m => m.MalfunctionId == id);
            if (malfunction == null)
            {
                return RedirectToAction("NotFoundPage");
            }

            return View(malfunction);
        }

        // GET: Malfunctions/Create
        [Authorize(Roles = "Admin,Janitor,Guide,SocialWorker")]
        public async Task<IActionResult> Create()
        {
            var apartments = from apt in _context.Apartment.Include(s => s.malfunctions)
                             select new { Value = apt.ApartmentId, Text = apt.Address };

            var users = from usr in _context.User.Include(s => s.malfunctions)
                        select new { Value = usr.Id, Text = usr.Email };

            var statuses = from Status stat in Enum.GetValues(typeof(Status))
                           select new { Value = (int)stat, Text = stat.ToString() };

            ViewData["all_appartments"] = new SelectList(await apartments.ToListAsync(), "Value", "Text");
            ViewData["all_users"] = new SelectList(await users.ToListAsync(), "Value", "Text");
            ViewData["statuses"] = new SelectList(statuses, "Value", "Text");

            return View();
        }

        // POST: Malfunctions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Janitor,Guide,SocialWorker")]
        public async Task<IActionResult> Create([Bind("CurrentApartment")]int CurrentApartment, [Bind("RequestedBy")]string RequestedBy, [Bind("MalfunctionId,Status,Title,Content,Resources,CurrentApartmentId,RequestedById")] Malfunction malfunction, IFormFile mFiles)
        {            
            if (ModelState.IsValid)
            {
                // Creating the query of the apartment
                var queryApt = from apt in _context.Apartment
                               where apt.ApartmentId == CurrentApartment
                               select apt;

                // Creating the query of the user
                var queryUsr = from usr in _context.User
                               where usr.Id == RequestedBy
                               select usr;

                // If the id of the apartment/user does not exist in DB
                if (!queryApt.Any() || !queryApt.Any())
                {
                    return View(malfunction);
                }

                // Adding the apartment to the malfunction to save
                var curApartment = queryApt.First();
                malfunction.CurrentApartment = curApartment;
                
                // Adding the user to the malfunction to save
                var curUser = queryUsr.First();
                malfunction.RequestedBy= curUser;

                // Adding the creation date and modification date
                malfunction.CreationDate = DateTime.Now;
                malfunction.ModifiedDate = DateTime.Now;

                // Uploading photo describing the malfunction
                SavePhoto(malfunction, mFiles);

                _context.Add(malfunction);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(malfunction);
        }

        private void SavePhoto(Malfunction malfunction, IFormFile mFiles)
        {
            // Sanity check apartment cant be null
            if (malfunction == null)
            {
                return;
            }

            // If there is no defined photo, set the default photo
            if (mFiles == null)
            {
                malfunction.Resources = "/images/malfunctions/default_malfunction_photo.jpg";
                return;
            }

            var fileName = Path.Combine(he.WebRootPath + "/images/malfunctions", Path.GetFileName(mFiles.FileName));
            malfunction.Resources = "/images/malfunctions/" + mFiles.FileName;

            // If the file does not exist already creating it
            if (!System.IO.File.Exists(fileName))
            {
                mFiles.CopyTo(new FileStream(fileName, FileMode.Create));
            }
        }

        // GET: Malfunctions/Edit/5
        [Authorize(Roles = "Admin,Janitor,Guide,SocialWorker")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFoundPage");
            }

            var malfunction = await _context.Malfunction.FindAsync(id);
            if (malfunction == null)
            {
                return RedirectToAction("NotFoundPage");
            }

            // Adding all the statuses to the viewData
            var statuses = from Status stat in Enum.GetValues(typeof(Status))
                select new { Value = (int)stat, Text = stat.ToString() };
            ViewData["statuses"] = new SelectList(statuses, "Value", "Text");

            return View(malfunction);
        }

        // POST: Malfunctions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Janitor,Guide,SocialWorker")]
        public async Task<IActionResult> Edit(string id, [Bind("MalfunctionId,Status,Title,Content,Resources")] Malfunction malfunction, IFormFile files)
        {
            if (id != malfunction.MalfunctionId)
            {
                return RedirectToAction("NotFoundPage");
            }

            if (ModelState.IsValid)
            {
                if (files != null)
                {
                    SavePhoto(malfunction, files);
                }

                if (!MalfunctionExists(malfunction.MalfunctionId))
                    return RedirectToAction("NotFoundPage");

                // Getting the malfunction from db
                var malfunctionToSave = _context.Malfunction.First(mal => mal.MalfunctionId == id);

                // Setting the new values
                malfunctionToSave.ModifiedDate = DateTime.Now;
                malfunctionToSave.Status = malfunction.Status;
                malfunctionToSave.Title = malfunction.Title;
                malfunctionToSave.Content = malfunction.Content;
                malfunctionToSave.Resources = malfunction.Resources;

                // Updating the new malfunction
                _context.Update(malfunctionToSave);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(malfunction);
        }

        // GET: Malfunctions/Delete/5
        [Authorize(Roles = "Admin,Janitor,Guide,SocialWorker")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFoundPage");
            }

            var malfunction = await _context.Malfunction
                .FirstOrDefaultAsync(m => m.MalfunctionId == id);
            if (malfunction == null)
            {
                return RedirectToAction("NotFoundPage");
            }

            return View(malfunction);
        }

        // POST: Malfunctions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var malfunction = await _context.Malfunction.FindAsync(id);
            _context.Malfunction.Remove(malfunction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MalfunctionExists(string id)
        {
            return _context.Malfunction.Any(e => e.MalfunctionId == id);
        }

        public IActionResult NotFoundPage()
        {
            return View();
        }
    }
}
