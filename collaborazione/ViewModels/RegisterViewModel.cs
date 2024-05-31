using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is Required")]
        [Display(Name = "Full Name")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string NameR { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [Display(Name = "Email")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid Email Format")]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "Account")]
        public string EmailR { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string PasswordR { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("PasswordR", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPasswordR { get; set; }

        [Display(Name = "Invited By")]
        [MaxLength(100, ErrorMessage = "Invited by cannot exceed 100 characters")]
        public string InvitedBy { get; set; }

        [Required(ErrorMessage = "Please Agree Privacy Policy & Terms")]
        [Display(Name = "I agree with the terms & policy.")]
        public bool AgreePrivacyPolicyTerms { get; set; }
    }
}
