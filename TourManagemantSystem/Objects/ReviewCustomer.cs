using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourManagemantSystem.Objects
{
    public class ReviewCustomer
    {
        public int ReviewId { get; set; }
        public int RegistrationId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PackageTitle { get; set; }
        public string Location { get; set; }
    }
}
