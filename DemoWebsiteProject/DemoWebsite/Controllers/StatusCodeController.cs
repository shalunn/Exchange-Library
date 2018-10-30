using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DemoWebsite.Controllers
{
    public class StatusCodeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public StatusCodeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // GET: /<controller>/
        [HttpGet("/StatusCode/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            _logger.LogError($"Unexpected Status Code: {statusCode}");
            return View(statusCode);
        }
    }
}
