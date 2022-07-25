using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourManagemantSystem.Models
{
    public class SendMailerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
