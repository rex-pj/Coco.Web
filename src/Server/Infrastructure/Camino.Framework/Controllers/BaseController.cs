﻿using Microsoft.AspNetCore.Mvc;

namespace Camino.Framework.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {

        }

        public IActionResult RedirectToErrorPage()
        {
            return RedirectToAction("Error", "Home");
        }

        public IActionResult RedirectToNotFoundPage()
        {
            return RedirectToAction("PageNotFound", "Home");
        }
    }
}
