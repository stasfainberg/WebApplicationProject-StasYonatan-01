using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TachzukanitBE.Data;

namespace TachzukanitBE.Controllers
{
    public class UsersController : Controller
    {
        private readonly TachzukanitDbContext _context;

        public UsersController(TachzukanitDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}