using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Bell.Controllers
{
    public class IdentityController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
    }
}