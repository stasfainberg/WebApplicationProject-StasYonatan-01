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
            // ---- Bar Graph ----
            // Query with Join and Group By- using address parameter
           var qBarGraph = from malfunctions in _context.Malfunction
                            join apartments in _context.Apartment on malfunctions.CurrentApartment equals apartments
                            where apartments.Address.Contains(address)
                            group malfunctions by malfunctions.CreationDate.Month into groupMalfunctions
                            select new
                            {
                                month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(groupMalfunctions.First().CreationDate.Month),
                                count = groupMalfunctions.Count()
                            };

            // Create JSON from list
            var malfunctionsBarJson = JsonConvert.SerializeObject(qBarGraph.ToList());
            ViewBag.Count = malfunctionsBarJson;


            // ---- Pie Graph ----
            var qPieGraph = from malfunction in _context.Malfunction
                            group malfunction by malfunction.Status into groupStatus
                            select new
                            {
                                status = ((Malfunction)groupStatus).Status.ToString(),
                                count = groupStatus.Count()
                            };

            // Create JSON from list
            var malfunctionsPieJson = JsonConvert.SerializeObject(qPieGraph.ToList());

            ViewBag.Status = malfunctionsPieJson;

            return View();
        }
    }
}