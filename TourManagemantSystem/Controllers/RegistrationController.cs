using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using TourManagemantSystem.Data.ChangedObjects;
using TourManagemantSystem.Helpers;
using TourManagemantSystem.Models;
using TourManagemantSystem.Objects;

namespace TourManagemantSystem.Controllers
{
    public class RegistrationController : Controller
    {
        private IConfiguration _config;
        private readonly UserManager<TourUser> userManager;
        private readonly SignInManager<TourUser> signInManager;
        private readonly IHostingEnvironment env;
        private readonly IEmailHelper emailHelper;

        public RegistrationController(IConfiguration configuration, UserManager<TourUser> userManager, SignInManager<TourUser> signInManager, IHostingEnvironment env, IEmailHelper emailHelper)
        {
            _config = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.env = env;
            this.emailHelper = emailHelper;
        }

        [Authorize]
        // GET: RegistrationController
        public ActionResult Index(int packageId, float packagePrice, string packageTitle)
        {
            RegistrationViewModel model = new RegistrationViewModel();

            model.PackageId = packageId;
            model.PackageTitle = packageTitle;
            model.Price = packagePrice;

            return View(model);
        }

        public async Task<IActionResult> SuccessfullyRegistered(RegistrationViewModel model) 
        {

            
            // get logged user id
            var userid = userManager.GetUserId(HttpContext.User);

            var isPayment = 0;
            var PackagePrices = model.Price;
            var CustomerPayment = model.Payment;
            if (PackagePrices == CustomerPayment)
            {
                isPayment = 1;
                var DbConnectionString = _config.GetConnectionString("TourManagementDBConnection");

                DBHelper helper = new DBHelper();
                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@PackageId", model.PackageId);
                sqlParameters[1] = new SqlParameter("@CustomerId", userid);
                sqlParameters[2] = new SqlParameter("@IsPayment", isPayment);

                helper.InsertData(DbConnectionString, "usp_InsertRegistration", sqlParameters);

                DBHelper helper1 = new DBHelper();

                var dt = helper1.GetData(DbConnectionString, "usp_GetTripDetails");

                TripDetailsViewModel model1 = new TripDetailsViewModel();

                List<TripDetails> tripDetails = new List<TripDetails>();


                foreach (DataRow item in dt.Rows)
                {
                    var p = new TripDetails()
                    {
                        UserName = item[0].ToString(),
                        Address = item[1].ToString(),
                        PhoneNumber = item[2].ToString(),
                        Email = item[3].ToString(),
                        Title = item[4].ToString(),
                        Description = item[5].ToString(),
                        Location = item[6].ToString(),
                        StartDate = item[7].ToString(),
                        StartTime = item[8].ToString(),
                        EndDate = item[9].ToString(),
                        EndTime = item[10].ToString(),
                        Price = item[11].ToString(),
                        RegistrationDate = item[14].ToString(),
                        RegistrationTime = item[15].ToString(),
                        PaymentStatus = item[16].ToString()

                    };
                    tripDetails.Add(p);
                }
                model1.AllTripDetails = tripDetails;

                //create a pdf file
                var fileName = model1.AllTripDetails[0].UserName + ".pdf";
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
                Document doc = new Document();
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                /*doc.Add(new Paragraph("Hello"));*/
                doc.Add(new Paragraph("Congratulations! Your trip registration has been successfully done. See your trip details in below."));
                doc.Add(new Paragraph("Customer Name: " + model1.AllTripDetails[0].UserName));
                doc.Add(new Paragraph("Address: " + model1.AllTripDetails[0].Address));
                doc.Add(new Paragraph("Phone Number: " + model1.AllTripDetails[0].PhoneNumber));
                doc.Add(new Paragraph("Location: " + model1.AllTripDetails[0].Location));
                doc.Add(new Paragraph("Description: " + model1.AllTripDetails[0].Description));
                doc.Add(new Paragraph("Price: " + model1.AllTripDetails[0].Price));
                doc.Add(new Paragraph("Start Date: " + model1.AllTripDetails[0].StartDate));
                doc.Add(new Paragraph("Start Time: " + model1.AllTripDetails[0].StartTime));
                doc.Add(new Paragraph("End Date: " + model1.AllTripDetails[0].EndDate));
                doc.Add(new Paragraph("End Time: " + model1.AllTripDetails[0].EndTime));
                doc.Add(new Paragraph("Registration Date: " + model1.AllTripDetails[0].RegistrationDate));
                doc.Add(new Paragraph("Registration Time: " + model1.AllTripDetails[0].RegistrationTime));
                doc.Add(new Paragraph("Payment Status: " + model1.AllTripDetails[0].PaymentStatus));
              


                var path = Path.Combine(env.WebRootPath, "EmailTemplates\\PackageRegistrationEmail.html");
                var contents = System.IO.File.ReadAllText(path);
                var emailBody = contents.Replace("{UserName}", model1.AllTripDetails[0].UserName)
                                        .Replace("{Address}", model1.AllTripDetails[0].Address)
                                        .Replace("{PhoneNumber}", model1.AllTripDetails[0].PhoneNumber)
                                        .Replace("{PackageName}", model1.AllTripDetails[0].Title)
                                        .Replace("{Location}", model1.AllTripDetails[0].Location)
                                        .Replace("{Description}", model1.AllTripDetails[0].Description)
                                        .Replace("{Price}", model1.AllTripDetails[0].Price)
                                        .Replace("{StartDate}", model1.AllTripDetails[0].StartDate)
                                        .Replace("{StartTime}", model1.AllTripDetails[0].StartTime)
                                        .Replace("{EndDate}", model1.AllTripDetails[0].EndDate)
                                        .Replace("{EndTime}", model1.AllTripDetails[0].EndTime)
                                        .Replace("{RegDate}", model1.AllTripDetails[0].RegistrationDate)
                                        .Replace("{RegTime}", model1.AllTripDetails[0].RegistrationTime)
                                        .Replace("{Status}", model1.AllTripDetails[0].PaymentStatus);

                doc.Close();
                //MailMessage mailMsg = new MailMessage();
                var Filepath = Path.Combine(@"D:\TourManagement\TourManagemantSystem\" + fileName);

                var path2 = Path.Combine(env.WebRootPath, "EmailTemplates\\PackageRegistrationEmail.html");
                //Attachment at = new Attachment(Filepath);
                //mailMsg.Attachments.Add(at);

                ////Sending trip details mail
                await emailHelper.SendAsync("learners.acdemy123@gmail.com",
                    model1.AllTripDetails[0].Email,
                    "Trip Registration Confirmation",
                      string.Format("Congratulations." +
                "We are very happy that you have completed our trip." +
                "Kindly give us your honest review about our trip." +
                "Download File <a href='{0}'>here</a> to check in", Url.ActionLink(Filepath)), Filepath
                    );

                //using (SmtpClient smtpClient = new SmtpClient())
                //{
                //    smtpClient.Timeout = 1000000;
                //    smtpClient.UseDefaultCredentials = false;
                //    await smtpClient.SendMailAsync(mailMsg);
                //}


                TempData["Message"] = "Package Registered Successfully!";
                return RedirectToAction("Index", "Package");
            }
            else 
            {
                ViewBag.msg = "Please complete the payment";
                ViewBag.PackageId = model.PackageId;
                return View("Index", model);
            }
            
            
        }

        public ActionResult UpdateRegistration(int registrationId) 
        {
            var DbConnectionString = _config.GetConnectionString("TourManagementDBConnection");

            DBHelper helper = new DBHelper();
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("@RegisterId", registrationId);

            var dt = helper.GetSelectedData(DbConnectionString, "usp_AvailablePackageForUpdate", sqlParameters);

            PackageViewModel model = new PackageViewModel();

            List<Package> packages = new List<Package>();


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
                    StartDate = item[6].ToString().Substring(0, 9),
                    StartTime = item[7].ToString(),
                    EndDate = item[8].ToString().Substring(0, 9),
                    EndTime = item[9].ToString()

                };
                packages.Add(p);
            }
            model.AllPackage = packages;

            ViewBag.Id = registrationId;
            return View(model);
        }

        public async Task<IActionResult> ChangePackage(RegistrationViewModel model, int registerId) 
        {
            var isPayment = 0;
            var PackagePrices = model.Price;
            var CustomerPayment = model.Payment;
            if (PackagePrices == CustomerPayment)
            {
                isPayment = 1;
                var DbConnectionString = _config.GetConnectionString("TourManagementDBConnection");

                DBHelper helper = new DBHelper();
                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@Id", registerId);
                sqlParameters[1] = new SqlParameter("@PackageId", model.PackageId);
                sqlParameters[2] = new SqlParameter("@IsPayment", isPayment);

                helper.InsertData(DbConnectionString, "usp_UpdateRegistration", sqlParameters);

                DBHelper helper1 = new DBHelper();

                var dt = helper1.GetData(DbConnectionString, "usp_GetTripDetails");

                TripDetailsViewModel model1 = new TripDetailsViewModel();

                List<TripDetails> tripDetails = new List<TripDetails>();


                foreach (DataRow item in dt.Rows)
                {
                    var p = new TripDetails()
                    {
                        UserName = item[0].ToString(),
                        Address = item[1].ToString(),
                        PhoneNumber = item[2].ToString(),
                        Email = item[3].ToString(),
                        Title = item[4].ToString(),
                        Description = item[5].ToString(),
                        Location = item[6].ToString(),
                        StartDate = item[7].ToString(),
                        StartTime = item[8].ToString(),
                        EndDate = item[9].ToString(),
                        EndTime = item[10].ToString(),
                        Price = item[11].ToString(),
                        RegistrationDate = item[12].ToString(),
                        RegistrationTime = item[13].ToString(),
                        PaymentStatus = item[14].ToString()

                    };
                    tripDetails.Add(p);
                }
                model1.AllTripDetails = tripDetails;

                var path = Path.Combine(env.WebRootPath, "EmailTemplates\\PackageRegistrationEmail.html");
                var contents = System.IO.File.ReadAllText(path);
                var emailBody = contents.Replace("{UserName}", model1.AllTripDetails[0].UserName)
                                        .Replace("{Address}", model1.AllTripDetails[0].Address)
                                        .Replace("{PhoneNumber}", model1.AllTripDetails[0].PhoneNumber)
                                        .Replace("{PackageName}", model1.AllTripDetails[0].Title)
                                        .Replace("{Location}", model1.AllTripDetails[0].Location)
                                        .Replace("{Description}", model1.AllTripDetails[0].Description)
                                        .Replace("{Price}", model1.AllTripDetails[0].Price)
                                        .Replace("{StartDate}", model1.AllTripDetails[0].StartDate)
                                        .Replace("{StartTime}", model1.AllTripDetails[0].StartTime)
                                        .Replace("{EndDate}", model1.AllTripDetails[0].EndDate)
                                        .Replace("{EndTime}", model1.AllTripDetails[0].EndTime)
                                        .Replace("{RegDate}", model1.AllTripDetails[0].RegistrationDate)
                                        .Replace("{RegTime}", model1.AllTripDetails[0].RegistrationTime)
                                        .Replace("{Status}", model1.AllTripDetails[0].PaymentStatus);

                //Sending trip details mail
                await emailHelper.SendAsync("learners.acdemy123@gmail.com",
                    model1.AllTripDetails[0].Email,
                    "Trip Package Update",
                    emailBody, null
                    );

                TempData["Message"] = "Package Update Successfully!";
                return RedirectToAction("Index", "Package");
            }
            else
            {
                ViewBag.msg = "Please complete the payment";
                ViewBag.PackageId = model.PackageId;
                return View("SuccessfullyUpdate", model);
            }

        }

        public ActionResult SuccessfullyUpdate(int packageId, int registerId, string packageTitle, float packagePrice) 
        {
            RegistrationViewModel model = new RegistrationViewModel();

            model.RegisterId = registerId;
            model.PackageId = packageId;
            model.PackageTitle = packageTitle;
            model.Price = packagePrice;

            return View(model);
        }


        public ActionResult DeleteRegistration(int registrationId)
        {
            var DBConnectionString = _config.GetConnectionString("TourManagementDBConnection");
            DBHelper helper = new DBHelper();

           
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("@RegId", registrationId);

            var data = helper.GetSelectedData(DBConnectionString, "usp_GetDeletedPackage", sqlParameters);


            RegisterPackageViewModel model = new RegisterPackageViewModel();

            List<CustomerRegisterPackage> RegisterPackage = new List<CustomerRegisterPackage>();


            foreach (DataRow item in data.Rows)
            {
                var p = new CustomerRegisterPackage()
                {

                    RegId = int.Parse(item[0].ToString()),
                    PackageTitle = item[1].ToString(),
                    Location = item[2].ToString(),
                    Price = item[3].ToString(),
                    RegDate = item[4].ToString(),
                };
                RegisterPackage.Add(p);
            }

            var TimeDiff = DateTime.Now - Convert.ToDateTime(RegisterPackage[0].RegDate);

            model.AllRegisterPackage = RegisterPackage;

            if(TimeDiff.Days > 0)
            {
                var Fine = (float.Parse(RegisterPackage[0].Price) * 10) / 100;
                var Refund = float.Parse(RegisterPackage[0].Price) - Fine;
                ViewBag.Fine = Fine;
                ViewBag.Refund = Refund;
            }
            else
            {
                var Fine = 0.0;
                var Refund = float.Parse(RegisterPackage[0].Price);
                ViewBag.Fine = Fine;
                ViewBag.Refund = Refund;
            }

            return View(model);
        }

        public ActionResult RemoveRegistration(int registrationId)
        {
            var DbConnectionString = _config.GetConnectionString("TourManagementDBConnection");

            DBHelper helper = new DBHelper();
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("@RegisterId", registrationId);

            helper.DeleteData(DbConnectionString, "usp_DeleteRegistration", sqlParameters);

            TempData["Message"] = "Register Delete Successfully!";
            return RedirectToAction("Index", "Customer");
        }


        // GET: RegistrationController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RegistrationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RegistrationController/Create
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

        // GET: RegistrationController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RegistrationController/Edit/5
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

        // GET: RegistrationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RegistrationController/Delete/5
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
