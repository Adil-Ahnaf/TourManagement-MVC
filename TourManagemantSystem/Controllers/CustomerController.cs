using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TourManagemantSystem.Data.ChangedObjects;
using TourManagemantSystem.Models;
using TourManagemantSystem.Objects;

namespace TourManagemantSystem.Controllers
{
    public class CustomerController : Controller
    {
        private IConfiguration _config;
        private readonly UserManager<TourUser> userManager;

        public CustomerController(IConfiguration configuration, UserManager<TourUser> userManager)
        {
            _config = configuration;
            this.userManager = userManager;
        }

        [Authorize]
        public ActionResult Index()
        {
            var DBConnectionString = _config.GetConnectionString("TourManagementDBConnection");
            DBHelper helper = new DBHelper();

            var userid = userManager.GetUserId(HttpContext.User);
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("@CustomerId", userid);

            var data = helper.GetSelectedData(DBConnectionString, "usp_RegisterPackageList", sqlParameters);


            RegisterPackageViewModel model = new RegisterPackageViewModel();

            List<CustomerRegisterPackage> RegisterPackage = new List<CustomerRegisterPackage>();


            foreach (DataRow item in data.Rows)
            {
                var p = new CustomerRegisterPackage()
                {

                    RegId = int.Parse(item[0].ToString()),
                    CheckInStatus = bool.Parse(item[1].ToString()),
                    PackageTitle = item[2].ToString(),
                    Description = item[3].ToString(),
                    Location = item[4].ToString(),
                    Price = item[5].ToString(),
                    StartDate = item[6].ToString().Substring(0, 9),
                    StartTime = item[7].ToString().Substring(0, 5),
                    EndDate = item[8].ToString().Substring(0, 9),
                    EndTime = item[9].ToString().Substring(0, 5),
                    RegDate = item[10].ToString().Substring(0, 9),
                    RegTime = item[11].ToString().Substring(0, 5),

                };
                RegisterPackage.Add(p);
            }
            model.AllRegisterPackage = RegisterPackage;

            ViewBag.Message = TempData["Message"];

            return View(model);
        }


        public ActionResult SignUp()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        // GET: CustomerController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CustomerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomerController/Create
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

        // GET: CustomerController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CustomerController/Edit/5
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

        // GET: CustomerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CustomerController/Delete/5
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
