using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TachzukanitBE.Data;
using Microsoft.AspNetCore.Authorization;

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

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
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
    }
}