using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace collaborazione.Utilities
{
    public class Helper
    {
        public bool EnumIsAny<T>(IEnumerable<T> data)
        {
            return data != null && data.Any();
        }

        public string RemoveAdditionalCharFromPhoneNumber(string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                return Regex.Replace(phoneNumber, "[^0-9+]+", "", RegexOptions.Compiled);
            }
            else
            {
                return phoneNumber;
            }
        }
    }
}
