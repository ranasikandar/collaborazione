using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.ViewModels
{
    public class UserHomeViewModel
    {
        public string UserFullName { get; set; }
        public string UserRole { get; set; }

        public ClientsStatusViewModel States { get; set; }
    }
}
