using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourManagemantSystem.Data.ChangedObjects
{
    public class TourUser : IdentityUser
    {
        public string Address { get; set; }

    }
}
