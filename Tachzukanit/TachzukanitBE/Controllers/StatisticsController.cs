﻿using System.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TachzukanitBE.Data;
using TachzukanitBE.Models;
using ProbabilityFunctions;
using System;
using Microsoft.EntityFrameworkCore;

namespace TachzukanitBE.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly TachzukanitDbContext _context;
        private const int TRESHOLD = 5;
        private DataTable table = new DataTable();
        private DataSet dataSet = new DataSet();
        private int[] malfCount = new int[12];
        
        public StatisticsController(TachzukanitDbContext context)
        {
            _context = context;
            table.Columns.Add("Method");
            table.Columns.Add("RoomNum", typeof(double));
            table.Columns.Add("Month", typeof(double));
            table.Columns.Add("Location", typeof(double));
        }

        public ActionResult Index(string address, string apartmentAddress, int month)
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

            if (month != 0 && !String.IsNullOrEmpty(apartmentAddress))
            {
                Apartment apartment = _context.Apartment.Include(m => m.malfunctions).FirstOrDefault(x => x.Address.Contains(apartmentAddress));
                InitiallizeTrainData(month);
                Classifier classifier = new Classifier();
                classifier.TrainClassifier(table);
                String name = classifier.Classify(new double[] { apartment.RoomsNumber, month, apartment.Longitude + apartment.Latitude });
                ViewData["classification"] = name;
            }

            return View();
        }
        
        [HttpGet]
        public JsonResult malfunctions_in_apartment(string address)
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
            return Json(JsonConvert.SerializeObject(qBarGraph.ToList()));
        }
        
        private void InitiallizeMalfPerMonth(Apartment apartment)
        {
            for (int i=0;i<12;i++)
            {
                malfCount[i] = 0;
            }
            foreach(Malfunction malf in apartment.malfunctions)
            {
                malfCount[malf.CreationDate.Month - 1] ++;
            }
        }

        private void InitiallizeTrainData(int month)
        {
            var apartments = _context.Apartment.Include(m => m.malfunctions).ToList();
            foreach(Apartment apartment in apartments)
            {
                double location = apartment.Latitude + apartment.Longitude;
                //for (int i = 1; i <= 12; i++)
                //{
                    InitiallizeMalfPerMonth(apartment);

                    if (malfCount[month - 1] < TRESHOLD)
                    {
                        table.Rows.Add("under", apartment.RoomsNumber, GetApartmentMalfByMonth(apartment, month), location);
                    }
                    else
                    {
                        table.Rows.Add("above", apartment.RoomsNumber, GetApartmentMalfByMonth(apartment, month), location);
                    }
                    
                //}
            }
            
        }
        private int GetApartmentMalfByMonth(Apartment apartment,int month)
        {
            int count = 0;

            if (apartment.malfunctions != null)
            {
                foreach (Malfunction malf in apartment.malfunctions)
                {
                    if (malf.CreationDate.Month == month)
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}