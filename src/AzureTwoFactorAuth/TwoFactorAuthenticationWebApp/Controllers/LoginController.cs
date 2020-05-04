using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TwoFactorAuthenticationWebApp.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Request.Method.Equals("Post", StringComparison.InvariantCultureIgnoreCase))
            {
                var userName = HttpContext.Request.Form["txtUserName"].ToString();
                var password = HttpContext.Request.Form["txtPassword"].ToString();

                // Simulate a valid user name and password authentication operation
                if(userName.Equals("admin", StringComparison.InvariantCultureIgnoreCase) && password.Equals("nabeel", StringComparison.InvariantCultureIgnoreCase))
                {
                    // valid user name and password.
                    return RedirectPermanent("/secondfactor");
                }
                return View("index", "invalid user name or password");
            }
            return View();
        }
    }
}