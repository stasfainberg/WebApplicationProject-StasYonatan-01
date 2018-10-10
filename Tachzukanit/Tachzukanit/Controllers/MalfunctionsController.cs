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
            
            ViewData["CurrentApartment"] = new SelectList(await apartments.ToListAsync(), "Value", "Text");
            ViewData["User"] = new SelectList(await users.ToListAsync(), "Value", "Text");

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
    }
}
