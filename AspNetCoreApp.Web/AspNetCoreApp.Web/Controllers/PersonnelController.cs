using BusinessLayer.Concrete;
using CoreLayer.Entities;
using CoreLayer.VM;
using DataAccessLayer.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreApp.Web.Controllers
{
    public class PersonnelController : Controller
    {

        private readonly IGENERICDAL<Personnel> _personnelContext;
        private readonly IGENERICDAL<Advance> advanceRepo;
        private readonly UserManager<Personnel> _userManager;

        public PersonnelController(IGENERICDAL<Personnel> personnelContext, IGENERICDAL<Advance> advanceRepo, UserManager<Personnel> userManager)
        {
            _personnelContext = personnelContext;
            this.advanceRepo = advanceRepo;
            _userManager = userManager;
        }

        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Personnel personnel = await _userManager.GetUserAsync(HttpContext.User); // Burası degistirilecek!!!

            return View(personnel);
        }

        [HttpGet]
        public async Task <IActionResult> Update(string id)
        {
            Personnel personnel = await _userManager.FindByIdAsync(id);

            //Personnel personnel = await _userManager.GetUserAsync(HttpContext.User);

            return View(personnel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Personnel personnel)
        {
            if (ModelState.IsValid)
            {
                if (personnel != null)
                {
                    await _userManager.UpdateAsync(personnel);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Update", "Güncelleme işlemi başarısız!");
                }
            }
            return View(personnel);
        }

        public async Task<IActionResult> Details()
        {
            Personnel personnel = await _userManager.GetUserAsync(HttpContext.User);
            return View(personnel);
        }

        public IActionResult Salary(Personnel personnel)
        {

            if (personnel.Advances!=null)
            {
                decimal totalSalary;
                var advance=advanceRepo.GetById(1);
                totalSalary = (decimal)(personnel.Salary - advance.AdvanceAmount); //Burayı kontrol et
                return View(totalSalary);

            }
           return View();
        }


    }
}
