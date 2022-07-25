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
using TourManagemantSystem.Helpers;
using TourManagemantSystem.Models;
using TourManagemantSystem.Objects;

namespace TourManagemantSystem.Controllers
{
    public class AdminController : Controller
    {
        private IConfiguration _config;
        private readonly UserManager<TourUser> userManager;
        private readonly SignInManager<TourUser> signInManager;
        private readonly IEmailHelper emailHelper;

        public AdminController(UserManager<TourUser> userManager, SignInManager<TourUser> signInManager, IConfiguration configuration, IEmailHelper emailHelper)
        {
            _config = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailHelper = emailHelper;
        }
        public ActionResult Index()
        {
            var DBConnectionString = _config.GetConnectionString("TourManagementDBConnection");
            DBHelper helper = new DBHelper();
            var dt = helper.GetData(DBConnectionString, "[usp_CheckInEmailBefore3day]");

            CheckInViewModel model = new CheckInViewModel();

            List<CheckIn> checkIns = new List<CheckIn>();


            foreach (DataRow item in dt.Rows)
            {
                var p = new CheckIn()
                {
                    RegisterId = int.Parse(item[0].ToString()),
                    CustomerName = item[1].ToString(),
                    Email = item[2].ToString(),
                    PackageTitle = item[3].ToString(),
                    StartDate = item[4].ToString().Substring(0, 9),
                    StartTime = item[5].ToString(),
                    EndDate = item[6].ToString().Substring(0, 9),
                    EndTime = item[7].ToString()

                };
                checkIns.Add(p);
            }
            model.AllCheckIn = checkIns;

            //ViewBag.Message = TempData["Message"];
            return View(model);
        }

        public IActionResult CheckInEmailBeforeOneDay() 
        {
            var DBConnectionString = _config.GetConnectionString("TourManagementDBConnection");
            DBHelper helper = new DBHelper();
            var dt = helper.GetData(DBConnectionString, "[usp_CheckInEmailBeforeOneDay]");

            CheckInViewModel model = new CheckInViewModel();

            List<CheckIn> checkIns = new List<CheckIn>();


            foreach (DataRow item in dt.Rows)
            {
                var p = new CheckIn()
                {
                    RegisterId = int.Parse(item[0].ToString()),
                    CustomerName = item[1].ToString(),
                    Email = item[2].ToString(),
                    PackageTitle = item[3].ToString(),
                    StartDate = item[4].ToString().Substring(0, 9),
                    StartTime = item[5].ToString(),
                    EndDate = item[6].ToString().Substring(0, 9),
                    EndTime = item[7].ToString()

                };
                checkIns.Add(p);
            }
            model.AllCheckIn = checkIns;

            //ViewBag.Message = TempData["Message"];
            return View(model);
        }

        public async Task<IActionResult> EmailBefore3Day(string customerName, string customerEmail, int registerId) 
        {

            var confirmationLink = Url.ActionLink("Index", "Customer");

            await emailHelper.SendAsync("learners.acdemy123@gmail.com",
                customerEmail,
                "Check You Tour Package",
                string.Format("Congratulations." +
                "We are very happy that you have registered our trip." +
                "Your trip will be start after 3 days." +
                "Kindly check in package and see the trip details." +
                "Click <a href='{0}'>here</a> to check in", confirmationLink), null
                );

            return RedirectToAction("Index","Admin");
        }

        public async Task<IActionResult> EmailBefore1Day(string customerName, string customerEmail, int registerId)
        {

            var confirmationLink = Url.ActionLink("Index", "Customer");

            await emailHelper.SendAsync("learners.acdemy123@gmail.com",
                customerEmail,
                "Check You Tour Package Last Reminder",
                string.Format("Congratulations." +
                "We are very happy that you have registered our trip." +
                "Your trip will be start after 1 days." +
                "Kindly check in package and see the trip details." +
                "Click <a href='{0}'>here</a> to check in", confirmationLink), null
                );

            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public async Task<IActionResult> SendSelectedEmail(int []regIdArray)
        {

            var array = regIdArray;

            var DBConnectionString = _config.GetConnectionString("TourManagementDBConnection");
            DBHelper helper = new DBHelper();
            SqlParameter[] sqlParameters = new SqlParameter[1];
            for (int i = 0; i < regIdArray.Length; i++)
            {
                sqlParameters[0] = new SqlParameter("@RegistrationId", regIdArray[i]);
                var dt = helper.GetSelectedData(DBConnectionString, "usp_SendSelectedEmail", sqlParameters);
                var confirmationLink = Url.ActionLink("Index", "Customer");

                DataRow item = dt.Rows[0];
                string customerEmail = item[0].ToString();

                await emailHelper.SendAsync("learners.acdemy123@gmail.com",
                customerEmail,
                "Check You Tour Package Last Reminder",
                string.Format("Congratulations." +
                "We are very happy that you have registered our trip." +
                "Your trip will be start after 1 days." +
                "Kindly check in package and see the trip details." +
                "Click <a href='{0}'>here</a> to check in", confirmationLink), null
                );
            }
            
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public ActionResult CheckInPackage(int registrationId)
        {
            var DbConnectionString = _config.GetConnectionString("TourManagementDBConnection");

            DBHelper helper = new DBHelper();
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("@RegisterId", registrationId);

            helper.CheckDataExit(DbConnectionString, "usp_UpdateCheckInStatus", sqlParameters);

            return RedirectToAction("Index", "Customer");
        }

        public ActionResult SendReview()
        {
            var DBConnectionString = _config.GetConnectionString("TourManagementDBConnection");
            DBHelper helper = new DBHelper();
            var dt = helper.GetData(DBConnectionString, "usp_SendReviewLink");

            //get all review customers list data
           
            DBHelper helper1 = new DBHelper();
            var data = helper.GetData(DBConnectionString, "usp_GetReviewCustomerList");

            ReviewCustomerViewModel model = new ReviewCustomerViewModel();

            List<ReviewCustomer> reviewCustomer = new List<ReviewCustomer>();


            foreach (DataRow item in data.Rows)
            {
                var p = new ReviewCustomer()
                {
                    ReviewId = int.Parse(item[0].ToString()),
                    RegistrationId = int.Parse(item[1].ToString()),
                    UserName = item[2].ToString(),
                    Email = item[3].ToString(),
                    PackageTitle = item[4].ToString(),
                    Location = item[5].ToString()        

                };
                reviewCustomer.Add(p);
            }
            model.AllReviewCustomer = reviewCustomer;

            
            return View(model);
        }

        public async Task<IActionResult> ReviewEmail(int ReviewId, int RegId, string Email, string PackageTitle, string Location)
        {

            var confirmationLink = Url.ActionLink("Review", "Admin", values: new { reviewId = ReviewId, regId=RegId, email=Email, packageTitle=PackageTitle, location=Location });

            await emailHelper.SendAsync("learners.acdemy123@gmail.com",
                Email,
                "Trip Review",
                string.Format("Congratulations." +
                "We are very happy that you have completed our trip." +        
                "Kindly give us your honest review about our trip." +
                "Click <a href='{0}'>here</a> to check in", confirmationLink), null
                );

            return RedirectToAction("SendReview", "Admin");
        }

        public ActionResult Review(int ReviewId, int RegId, string Email, string PackageTitle, string Location)
        {
            
            ViewBag.RegId = RegId;
            ViewBag.PackageTitle = PackageTitle;
            ViewBag.Location = Location;
            return View();
        }

        public ActionResult SubmitReview(ReviewViewModel model)
        {
            var DbConnectionString = _config.GetConnectionString("TourManagementDBConnection");

            DBHelper helper = new DBHelper();
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@RegisterId", model.RegId);
            sqlParameters[1] = new SqlParameter("@Review", model.Review);


            helper.InsertData(DbConnectionString, "usp_InsertReview", sqlParameters);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult CustomerReview()
        {
            var DbConnectionString = _config.GetConnectionString("TourManagementDBConnection");

            DBHelper helper = new DBHelper();
            var data = helper.GetData(DbConnectionString, "usp_ShowAllReview");

            ReviewCustomerViewModel model = new ReviewCustomerViewModel();

            List<ReviewCustomer> reviewCustomer = new List<ReviewCustomer>();


            foreach (DataRow item in data.Rows)
            {
                var p = new ReviewCustomer()
                {
                    PackageTitle = item[0].ToString(),
                    UserName = item[1].ToString(),
                    Location = item[2].ToString()

                };
                reviewCustomer.Add(p);
            }
            model.AllReviewCustomer = reviewCustomer;

            return View(model);
        }

        public ActionResult NextDayTourList()
        {
            var DBConnectionString = _config.GetConnectionString("TourManagementDBConnection");
            DBHelper helper = new DBHelper();
            var dt = helper.GetData(DBConnectionString, "[usp_TourManagerOneDayBeforeCustomerInfo]");

            CheckInViewModel model = new CheckInViewModel();

            List<CheckIn> checkIns = new List<CheckIn>();


            foreach (DataRow item in dt.Rows)
            {
                var p = new CheckIn()
                {
                    CustomerName = item[0].ToString(),
                    Email = item[1].ToString(),
                    PackageTitle = item[2].ToString(),
                    EndDate = item[3].ToString(),
                    StartDate = item[4].ToString().Substring(0, 9),
                    StartTime = item[5].ToString()
                };
                checkIns.Add(p);
            }
            model.AllCheckIn = checkIns;

            //ViewBag.Message = TempData["Message"];
            return View(model);
        }
        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
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

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
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

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
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
