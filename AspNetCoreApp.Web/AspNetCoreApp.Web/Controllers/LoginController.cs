using AspNetCoreApp.Web.Models;
using BusinessLayer.ValidationRules;
using CoreLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreApp.Web.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly SignInManager<Personnel> _signInManager;
        private readonly UserManager<Personnel> _userManager;

        public LoginController(SignInManager<Personnel> signInManager, UserManager<Personnel> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(UserSignInViewModel userSign)
        {
            if (ModelState.IsValid)
            {
                Personnel user = await _userManager.FindByEmailAsync(userSign.email);

                

                if (user != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, userSign.password, true, true);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Personnel");
                    }
                    else
                    {
                        ModelState.AddModelError("Parola ve kullanici hatali", "Girdiginiz kullanci adi veya parola hatali");
                    }
                }
                else
                {
                    ModelState.AddModelError("Parola ve kullanici hatali", "Girdiginiz kullanci adi veya parola hatali");
                }
            }
            return View(userSign);
        }
    }

}
