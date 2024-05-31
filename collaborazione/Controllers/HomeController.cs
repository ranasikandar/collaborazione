using collaborazione.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.Controllers
{
    public class HomeController : Controller
    {

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string fname)
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult TermsAndConditions()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PrivacyPolicyPartial()
        {
            return PartialView("_PrivacyPolicyPartial");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult TermsAndConditionsPartial()
        {
            return PartialView("_TermsAndConditionsPartial");
        }


    }
}
