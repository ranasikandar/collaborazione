using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.Controllers
{
    public class ErrorController : Controller
    {
        #region CTOR
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> _logger)
        {
            this.logger = _logger;
        }

        #endregion

        [Route("Error/{statusCode}")]
        [AllowAnonymous]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            string _view = "Error500";

            switch (statusCode)
            {
                case 404:
                    logger.LogWarning($"404 Error Occured. Path = {statusCodeResult.OriginalPath}" +
                        $" and QueryString = {statusCodeResult.OriginalQueryString}");
                    _view = "Error404";
                    break;
                case 500:
                    logger.LogError($"500 Error Occured. Path = {statusCodeResult.OriginalPath}" +
                        $" and QueryString = {statusCodeResult.OriginalQueryString}");
                    _view = "Error500";
                    break;
            }

            return View(_view);
        }

        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionDetails == null)
            {
                logger.LogError($"UnKnown Error");
            }
            else
            {
                logger.LogError($"The path {exceptionDetails.Path} threw an exception " +
                $"{exceptionDetails.Error}");
                //$"{((exceptionDetails.Error == null) ? new Exception("UnKnown Error") : exceptionDetails.Error)}");
            }

            return View("Error500");
        }

        //[Route("Error/NoJS")]
        //[AllowAnonymous]
        //public IActionResult ErrorNoJS()
        //{
        //    return View();
        //}
    }
}
