using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourManagemantSystem.Data.ChangedObjects;

namespace TourManagemantSystem.Models
{
    public class RegistrationViewModel
    {
        public int PackageId { get; set; }
        public int RegisterId { get; set; }
        public string PackageTitle { get; set; }
        public int CustomerId { get; set; }
        public float Price { get; set; }
        public float Payment { get; set; }
    }
}
