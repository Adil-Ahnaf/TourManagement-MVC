using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TourManagemantSystem.Models;
using TourManagemantSystem.Objects;

namespace TourManagemantSystem.Controllers
{
    public class PackageController : Controller
    {
        private IConfiguration _config;

        public PackageController(IConfiguration configuration)
        {
            _config = configuration;
        }
        // GET: PackageController
        public ActionResult Index()
        {
            var DBConnectionString = _config.GetConnectionString("TourManagementDBConnection");
            DBHelper helper = new DBHelper();
            var dt = helper.GetData(DBConnectionString, "usp_AvaliablePackages");

            PackageViewModel model = new PackageViewModel();

            List<Package> packages = new List<Package>();

            var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Package List");

            List<string> header_list = new List<string>();
            header_list.Add("Package Id");
            header_list.Add("Title");
            header_list.Add("Description");
            header_list.Add("Location");
            header_list.Add("Price");
            header_list.Add("Seat");
            header_list.Add("Start Date");
            header_list.Add("Start Time");
            header_list.Add("End Date");
            header_list.Add("End Time");

            //worksheet.FirstRow().Value = header_list;

            int i = 1;
            for (int k = 0; k < header_list.Count; k++)
            {
                worksheet.Cell(i, k + 1).Value = header_list[k];
            }

            i = 2; 
            int j = 1;
            foreach (DataRow item in dt.Rows)
            {
                var p = new Package()
                {
                    Id = int.Parse(item[0].ToString()),
                    Title = item[1].ToString(),
                    Description = item[2].ToString(),
                    Location = item[3].ToString(),
                    Price = item[4].ToString(),
                    Seat = int.Parse(item[5].ToString()),
                    StartDate = item[6].ToString().Substring(0,9),
                    StartTime = item[7].ToString(),
                    EndDate = item[8].ToString().Substring(0, 9),
                    EndTime = item[9].ToString()
                };
                packages.Add(p);

                worksheet.Cell(i,j++).Value = int.Parse(item[0].ToString());
                worksheet.Cell(i,j++).Value = item[1].ToString();
                worksheet.Cell(i,j++).Value = item[2].ToString();
                worksheet.Cell(i,j++).Value = item[3].ToString();
                worksheet.Cell(i,j++).Value = item[4].ToString();
                worksheet.Cell(i,j++).Value = int.Parse(item[5].ToString());
                worksheet.Cell(i,j++).Value = item[6].ToString();
                worksheet.Cell(i,j++).Value = item[7].ToString();
                worksheet.Cell(i,j++).Value = item[8].ToString();
                worksheet.Cell(i,j++).Value = item[9].ToString();
                i++; j = 1;
            }
            model.AllPackage = packages;

            workbook.SaveAs("Available_Package_List.xlsx");

            ViewBag.Message = TempData["Message"];
            return View(model);
        }

        [HttpPost]
        public IActionResult Export()
        {
            
            var contentType = "application/octet-stream"; 
            var fileName = "Available_Package_List.xlsx";
            var Filepath = Path.Combine(@"G:\C#\TourManagementFinal\TourManagement\TourManagemantSystem\" + fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(Filepath);
            return File(fileBytes, contentType, fileName);

            /*DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[4] { new DataColumn("CustomerId"),
                                        new DataColumn("ContactName"),
                                        new DataColumn("City"),
                                        new DataColumn("Country") });

            var customers = from customer in this.Context.Customers.Take(10)
                            select customer;

            foreach (var customer in customers)
            {
                dt.Rows.Add(customer.CustomerID, customer.ContactName, customer.City, customer.Country);
            }*/

            /*using (XLWorkbook wb = new XLWorkbook())
            {
                *//*wb.Worksheets.Add(dt);*//*
                using (MemoryStream stream = new MemoryStream())
                {
                    *//*wb.SaveAs(stream);*//*
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", @"TourManagemantSystem\Available_Package_List.xlsx");
                }
            }*/
        }

        // GET: PackageController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PackageController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PackageController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PackageController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PackageController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PackageController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PackageController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
