using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourManagemantSystem.Objects
{
    public class CheckIn
    {
        public int RegisterId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PackageTitle { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
    }
}
