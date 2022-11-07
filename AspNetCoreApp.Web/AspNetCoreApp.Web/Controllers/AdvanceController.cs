using BusinessLayer.Abstract;
using CoreLayer.Entities;
using CoreLayer.VM;
using DataAccessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreApp.Web.Controllers
{
    public class AdvanceController : Controller
    {
        private readonly IGenericService<Advance> _advanceService;
        private readonly IGenericService<Personnel> _personnelService;

        public AdvanceController(IGenericService<Advance> advanceService, IGenericService<Personnel> personnelService)
        {
            _advanceService = advanceService;
            _personnelService = personnelService;
        }
        public IActionResult Index()
        {
            var advance = _advanceService.GetListAll();
            return View(advance);
        }

        [HttpGet]
        public IActionResult Add()
        {
            PersonnelAdvanceVM vm = new PersonnelAdvanceVM();
            vm.Advance = new Advance();
           
             vm.Personnels = _personnelService.GetListAll();
             return View(vm);
            

         
            
        }

        [HttpPost]
        public IActionResult Add(PersonnelAdvanceVM vm)
        {
            Advance newAdvance = new Advance()
            {
                AdvanceAmount = vm.Advance.AdvanceAmount,
                CreationDate = vm.Advance.CreationDate,
                Approval = vm.Advance.Approval,
                ApprovalDate = vm.Advance.ApprovalDate,
                Currency = vm.Advance.Currency,
                Description = vm.Advance.Description,

                PersonnelID = vm.Advance.PersonnelID

            };
            if (ModelState.IsValid)
            {
                _advanceService.Insert(newAdvance);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Avans eklenemedi";
                vm.Personnels = _personnelService.GetListAll();
                return View(vm); 
            }
              
              
        }
    }
}
