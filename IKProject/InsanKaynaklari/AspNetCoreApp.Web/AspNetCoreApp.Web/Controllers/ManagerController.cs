﻿using AspNetCoreApp.Web.Models;
using BusinessLayer.Abstract;
using CoreLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreApp.Web.Controllers
{
    public class ManagerController : Controller
    {
        private readonly UserManager<Personnel> userManager;
        private readonly IGenericService<Personnel> personnelService;
        private readonly IPersonnelService personnelManager;

        public ManagerController(UserManager<Personnel> userManager,IGenericService<Personnel> personnelService,IPersonnelService personnelManager)
        {
            this.userManager = userManager;
            this.personnelService = personnelService;
            this.personnelManager = personnelManager;
        }
        public IActionResult Index()
        {
            List<Personnel> personnels = personnelManager.GetAllPersonelsWithDepartment();
            return View(personnels);
        }

        [HttpPost]
        public IActionResult Index(string name, string surname)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(surname))
            {
                List<Personnel> personnels = personnelManager.GetAllPersonelsWithDepartment();
                return View(personnels);
            }
            if (name != null)
            {
                List<Personnel> personnels = personnelManager.GetAllPersonelsWithDepartmentFilter(a=>a.Name==name);
                return View(personnels);
            }
            if (name != null && surname != null)
            {
                List<Personnel> personnels = personnelManager.GetAllPersonelsWithDepartmentFilter(a => a.Name == name && a.Surname==surname);
                return View(personnels);
            }
            if (surname != null)
            {
                List<Personnel> personnels = personnelManager.GetAllPersonelsWithDepartmentFilter(a => a.Surname == surname);
                return View(personnels);
            }

            return View(null);
           

        }

        public async Task<IActionResult> CreatePersonnel(UserSignUpViewModel userSignUp)
        {
            if (ModelState.IsValid) 
            { 
              Personnel personnel = new Personnel()
              {
                Email = userSignUp.Mail,
                Name = userSignUp.Name,
                Surname = userSignUp.Surname,
                SecondSurname = userSignUp.SecondSurname,
                MiddleName = userSignUp.MiddleName,
                IdentityNumber = userSignUp.IdentityNumber,
                Job = userSignUp.Job,
                HireDate = userSignUp.HireDate,
                Address = userSignUp.Address,
                IsActive = true,
                BirthDate=userSignUp.BirthDate,
                DepartmentID=userSignUp.DepartmentID,

              };

              var result = await userManager.CreateAsync(personnel, userSignUp.Password);

              if (result.Succeeded)
              {
                return RedirectToAction("Index");
              }
              else
              {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                    return View(userSignUp);
              }

             
            }
            else
            {
                ViewBag.Message = "Personel eklenemedi";
                return View(userSignUp);
            }
        }
    }
}
