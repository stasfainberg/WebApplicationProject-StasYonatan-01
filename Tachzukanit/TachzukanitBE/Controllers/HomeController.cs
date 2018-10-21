﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TachzukanitBE.Data;
using TachzukanitBE.Models;

namespace TachzukanitBE.Controllers
{
    public class HomeController : Controller
    {
        private readonly TachzukanitDbContext _context;

        public HomeController(TachzukanitDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var apartments = from apt in _context.Apartment select apt;
            return View(apartments);
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
