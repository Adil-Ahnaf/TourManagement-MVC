using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourManagemantSystem.Helpers
{
    public interface IEmailHelper
    {
        Task SendAsync(string from, string to, string subject, string body, string filePath);
    }
}
