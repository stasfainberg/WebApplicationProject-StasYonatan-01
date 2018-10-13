using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tachzukanit.Models;

namespace Tachzukanit.Controllers
{
    public class MalfunctionsController : Controller
    {
        private readonly TachzukanitContext _context;

        public MalfunctionsController(TachzukanitContext context)
        {
            _context = context;
        }

        // GET: Malfunctions
        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.Malfunction.Include(p => p.CurrentApartment).Include(ps => ps.RequestedBy);
            return View(await databaseContext.ToListAsync());
        }

        // GET: Malfunctions/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var malfunction = await _context.Malfunction
                .Include(p => p.RequestedBy).Include(ps => ps.CurrentApartment)
                .SingleOrDefaultAsync(m => m.MalfunctionId == id);
            if (malfunction == null)
            {
                return NotFound();
            }

            return View(malfunction);
        }

        // GET: Malfunctions/Create
        public async Task<IActionResult> Create()
        {            
            var apartments = from apt in _context.Apartment.Include(s => s.malfunctions)
                    select new { Value = apt.ApartmentId, Text = apt.Address};

            var users = from usr in _context.User.Include(s => s.malfunctions)
                    select new { Value = usr.UserId, Text = usr.Name };
            
            ViewData["all_appartments"] = new SelectList(await apartments.ToListAsync(), "Value", "Text");
            ViewData["all_users"] = new SelectList(await users.ToListAsync(), "Value", "Text");

            return View();
        }

        // POST: Malfunctions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MalfunctionId,Status,Title,Content,Resources,CreationDate,ModifiedDate,CurrentApartment")] Malfunction malfunction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(malfunction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(malfunction);
        }

        // GET: Malfunctions/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var malfunction = await _context.Malfunction.SingleOrDefaultAsync(m => m.MalfunctionId == id);
            if (malfunction == null)
            {
                return NotFound();
            }
            return View(malfunction);
        }

        // POST: Malfunctions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MalfunctionId,Status,Title,Content,Resources,CreationDate,ModifiedDate")] Malfunction malfunction)
        {
            if (id != malfunction.MalfunctionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(malfunction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MalfunctionExists(malfunction.MalfunctionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(malfunction);
        }

        // GET: Malfunctions/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var malfunction = await _context.Malfunction
                .SingleOrDefaultAsync(m => m.MalfunctionId == id);
            if (malfunction == null)
            {
                return NotFound();
            }

            return View(malfunction);
        }

        // POST: Malfunctions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var malfunction = await _context.Malfunction.SingleOrDefaultAsync(m => m.MalfunctionId == id);
            _context.Malfunction.Remove(malfunction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MalfunctionExists(string id)
        {
            return _context.Malfunction.Any(e => e.MalfunctionId == id);
        }

        // GET: Malfunctions/Search
        public ActionResult Search(String status = null, String title= null, String content = null,
                                    String createDate = null, String modifeyDate = null,
                                    String roomNum = null, String userName = null)
        {
            // Get the list of all apartments
            ViewBag.Malfunctions = new SelectList(_context.Malfunction.Select(i => i.Title));

            var returnDataQuery = _context.Malfunction.Include(r => r.CurrentApartment).Include(r => r.RequestedBy).Select(r => r);

            // Check if status is not null
            if (!String.IsNullOrEmpty(status))
            {
                returnDataQuery = returnDataQuery.Where(i => ((int)i.Status).ToString() == status);
            }

            // Check if the title is not null
            if (!String.IsNullOrEmpty(title))
            {
                returnDataQuery = returnDataQuery.Where(i => i.Title.Contains(title));
            }

            // Check if the content is not null
            if (!String.IsNullOrEmpty(content))
            {
                returnDataQuery = returnDataQuery.Where(i => i.Content.Contains(content));
            }

            // Check if the creation date is not null
            if (!String.IsNullOrEmpty(createDate))
            {
                returnDataQuery = returnDataQuery.Where(i => i.CreationDate >= DateTime.Parse(createDate));
            }

            // Check if the modifey date is not null
            if (!String.IsNullOrEmpty(modifeyDate))
            {
                returnDataQuery = returnDataQuery.Where(i => i.ModifiedDate <= DateTime.Parse(modifeyDate));
            }

            // Check if the room number is not null
            if (!String.IsNullOrEmpty(modifeyDate))
            {
                returnDataQuery = returnDataQuery.Where(i => i.CurrentApartment.RoomsNumber == roomNum);
            }

            // Check if the user name is not null
            if (!String.IsNullOrEmpty(userName))
            {
                returnDataQuery = returnDataQuery.Where(i => i.RequestedBy.Name.Equals(userName));
            }

            return View(returnDataQuery.ToList());
        }
    }
}
