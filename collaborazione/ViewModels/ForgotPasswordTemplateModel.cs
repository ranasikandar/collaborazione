using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.ViewModels
{
    public class ForgotPasswordTemplateModel
    {
        public string UserId { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }
    }
}
