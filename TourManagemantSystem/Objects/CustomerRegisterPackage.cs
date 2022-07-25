using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourManagemantSystem.Objects
{
    public class CustomerRegisterPackage
    {
        public int RegId { get; set; }
        public bool CheckInStatus { get; set; }
        public string PackageTitle { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Price { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public string RegDate { get; set; }
        public string RegTime { get; set; }
    }
}
