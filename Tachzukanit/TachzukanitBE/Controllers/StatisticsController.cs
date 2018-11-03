using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TachzukanitBE.Data;
using TachzukanitBE.Models;

namespace TachzukanitBE.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly TachzukanitDbContext _context;

        public StatisticsController(TachzukanitDbContext context)
        {
            _context = context;
        }
        public ActionResult Index(string address)
        {
            var q = from malfunctions in _context.Malfunction
                    join apartments in _context.Apartment on malfunctions.CurrentApartment equals apartments
                    where apartments.Address.Equals(address)
                    group malfunctions by malfunctions.CreationDate.Month into groupMalfunctions
                    select new
                    {
                        month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(groupMalfunctions.First().CreationDate.Month),
                        count = groupMalfunctions.Count()
                    };
            //CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(((Malfunction)groupMalfunctions).CreationDate.Month)
            var malfunctionCount = new List<dynamic>();

            foreach (var malfunctions in q)
            {
                malfunctionCount.Add(malfunctions);
            }

            var malfunctionsJson = JsonConvert.SerializeObject(malfunctionCount);
            ViewBag.Count = malfunctionsJson;
            
            return View();
        }
    }
}