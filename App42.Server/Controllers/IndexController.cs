using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace App42.Server.Controllers
{
    [Route("")]
    public class IndexController : Controller
    {
        [Route("{*url}", Order = 1)]
        public IActionResult Index()
        {
#if RELEASE
            bool isDebug = false;
#else
            bool isDebug = true;
#endif
            return View(isDebug);
        }
    }
}
