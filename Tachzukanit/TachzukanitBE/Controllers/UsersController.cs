﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TachzukanitBE.Data;
using Microsoft.AspNetCore.Authorization;
using TachzukanitBE.Models;
using Microsoft.AspNetCore.Identity;

namespace TachzukanitBE.Controllers
{
    public class UsersController : Controller
    {
        private readonly TachzukanitDbContext _context;

        public UsersController(TachzukanitDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(String searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                return View(await _context.User.ToListAsync());
            }

            return View(await _context.User.Where(a => a.Address.Contains(searchString)).ToListAsync());
        }

        public async Task<IActionResult> Details(String id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(String id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            //if (_context.Users..Current.User.Identity.GetUserId())
            //{

            //}

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(String id, [Bind("Id,FullName,Email,PhoneNumber,Address")] User user)
        {
            if (!id.Equals(user.Id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    User dbUser = (User)_context.Users.First(u => u.Id == id);
                    dbUser.PhoneNumber = user.PhoneNumber;
                    dbUser.FullName = user.FullName;
                    dbUser.Address = user.Address;
                    _context.Update(dbUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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

            return View(user);
        }

        private bool UserExists(String id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(String id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(String id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }

}