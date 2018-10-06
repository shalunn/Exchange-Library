using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoWebsite.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DemoWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmployeeDataContext _db;

        public HomeController(EmployeeDataContext db)
        {
            _db = db;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {           
            var employees = _db.Employees.ToArray();

            return View(employees);
        }
    }
}
