using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.ServiceFabric.Test.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Microsoft.Extensions.Caching.ServiceFabric.Test.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IDistributedCache _distributedCache;

        public HomeController(IDistributedCache distributedCache)
        {
            //  _logger = logger;
            _distributedCache = distributedCache;
        }

        public IActionResult Index()
        {
            var sessionValue = "TestSession";
            var checkValue = HttpContext.Session.GetString(sessionValue);

            if (string.IsNullOrEmpty(checkValue))
            {
                HttpContext.Session.SetString(sessionValue, sessionValue + Guid.NewGuid().ToString());
            }

            ViewData[sessionValue] = HttpContext.Session.GetString(sessionValue);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
