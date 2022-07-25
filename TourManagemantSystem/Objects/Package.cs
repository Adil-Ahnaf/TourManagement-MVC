using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourManagemantSystem.Objects
{
    public class Package
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }
        public string Price { get; set; }

        public int Seat { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
    }
}
