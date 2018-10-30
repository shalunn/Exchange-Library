using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DemoWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmployeeDataContext _db;
        private MailingService mailing;
        protected readonly ILogger<HomeController> _logger;

        public HomeController(EmployeeDataContext db, IOptions<SmtpConfig> SmtpConfig, ILogger<HomeController> logger = null)
        {
            _db = db;
            mailing = new MailingService(SmtpConfig);            

            if (logger != null)
            {
                _logger = logger;
            }   
        }

        // GET: /<controller>/
        public IActionResult Index()
        {           
            var employees = _db.Employees.ToArray();

            return View(employees);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(EmailFormModel model)
        {
            if (ModelState.IsValid)
            {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                body = string.Format(body, model.FromName, model.FromEmail, model.Message);
                mailing.SendEmail(model.FromEmail, "Your email subject", model.Message);

                return RedirectToAction("Sent");
            }
            return View(model);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Sent()
        {
            return View();
        }
    }
}
