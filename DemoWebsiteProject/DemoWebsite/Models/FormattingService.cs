using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebsite.Models
{
    public class FormattingService
    {
        public string AsReadableDate(DateTime date)
        {
            return date.ToString("d");
        }

        public string AsReadableSalary(double salary)
        {
            return salary.ToString("C", new CultureInfo("en-US")); ;
        }
    }
}
