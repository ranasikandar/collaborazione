using collaborazione.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.ViewModels
{
    public class AppUserVM:ApplicationUser
    {
        public string Role { get; set; }
        public int ClientsAdded { get; set; }
    }
}
