using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.Models
{
    public class ApplicationUser: IdentityUser
    {
        [MaxLength(256, ErrorMessage = "Name cannot exceed 256 characters")]
        public string Name { get; set; }

        //just delete email,password and set isdeleted 1
        public bool? IsDeleted { get; set; }

        //push noti fields
        public string EndPoint { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }

        [MaxLength(100, ErrorMessage = "Invited by cannot exceed 100 characters")]
        public string InvitedBy { get; set; }
    }
}
