using collaborazione.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.ViewModels
{
    public class NotifyViewModel
    {
        [Display(Name = "Select Users")]
        public List<ApplicationUser> Users { get; set; }

        [Required(ErrorMessage = "Select one or more users")]
        [Display(Name = "Select Users")]
        public List<string> SelectedUser { get; set; }

        public string Message { get; set; }
    }
}
