using AspNetCoreApp.Web.Models;
using BusinessLayer.Abstract;
using CoreLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Mvc;

namespace AspNetCoreApp.Web.Controllers
{
    [Authorize(Roles = "Manager")]

    public class ManagerController : Controller
    {
        private readonly UserManager<Personnel> userManager;
        private readonly IGenericService<Personnel> personnelService;
        private readonly IPersonnelService personnelManager;
        private readonly IGenericService<Department> departmentService;
        private readonly IPasswordHasher<Personnel> passwordHasher;
        private readonly IGenericService<Advance> advanceService;
        private readonly IGenericService<Permit> permitService;
        private readonly IPermitService _permitService;
        private readonly IGenericService<Expense> expenseService;


        public ManagerController(UserManager<Personnel> userManager, IGenericService<Personnel> personnelService, IPersonnelService personnelManager, IGenericService<Department> departmentService, IPasswordHasher<Personnel> passwordHasher, IGenericService<Advance> advanceService, IPermitService permitService, IGenericService<Expense> expenseService, IGenericService<Permit> permitService1)
        {
            this.userManager = userManager;
            this.personnelService = personnelService;
            this.personnelManager = personnelManager;
            this.departmentService = departmentService;
            this.passwordHasher = passwordHasher;
            this.advanceService = advanceService;
            _permitService = permitService;
            this.permitService = permitService1;
            this.expenseService = expenseService;
        }
        public IActionResult Index(int page = 1)
        {
            PagedList<Personnel> personnels = (PagedList<Personnel>)personnelManager.GetAllPersonelsWithDepartment().ToPagedList(page, 6);
            return View(personnels);
        }
        [HttpPost]
        public IActionResult Index(string name, string surname, int page = 1)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(surname))
            {
                PagedList<Personnel> personnels = (PagedList<Personnel>)personnelManager.GetAllPersonelsWithDepartment().ToPagedList(page, 4);
                return View(personnels);
            }
            if (name != null)
            {
                PagedList<Personnel> personnels = (PagedList<Personnel>)personnelManager.GetAllPersonelsWithDepartmentFilter(a => a.Name == name).ToPagedList(page, 4);
                return View(personnels);
            }
            if (name != null && surname != null)
            {
                PagedList<Personnel> personnels = (PagedList<Personnel>)personnelManager.GetAllPersonelsWithDepartmentFilter(a => a.Name == name && a.Surname == surname).ToPagedList(page, 4);
                return View(personnels);
            }
            if (surname != null)
            {
                PagedList<Personnel> personnels = (PagedList<Personnel>)personnelManager.GetAllPersonelsWithDepartmentFilter(a => a.Surname == surname).ToPagedList(page, 4);
                return View(personnels);
            }
            return View(null);
        }
        [HttpGet]
        public IActionResult CreatePersonnel()
        {
            UserSignUpViewModel userSignUp = new UserSignUpViewModel();
            userSignUp.Departments = departmentService.GetListAll();
            return View(userSignUp);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePersonnel(UserSignUpViewModel userSignUp)
        {
            if (ModelState.IsValid)
            {
                Personnel personnel = new Personnel();

                personnel.Email = userSignUp.Name.ToLower() + "." + userSignUp.Surname.ToLower() + "@bilgeadamboost.com";
                personnel.Name = userSignUp.Name;
                personnel.Surname = userSignUp.Surname;
                personnel.SecondSurname = userSignUp.SecondSurname;
                personnel.MiddleName = userSignUp.MiddleName;
                personnel.IdentityNumber = userSignUp.IdentityNumber;
                personnel.Job = userSignUp.Job;
                personnel.HireDate = userSignUp.HireDate;
                personnel.Address = userSignUp.Address;
                personnel.IsActive = true;
                personnel.BirthDate = userSignUp.BirthDate;
                personnel.DepartmentID = userSignUp.DepartmentID;
                personnel.PhoneNumber = userSignUp.PhoneNumber;
                personnel.Gender = userSignUp.Gender;
                personnel.UserName = personnel.Email;
                personnel.PlaceOfBirth = userSignUp.PlaceOfBirth;

                string Role = "Personel";

                Guid guid = Guid.NewGuid();
                string newPassword = guid.ToString().Substring(0, 8);
                SmtpClient client = new SmtpClient();
                client.Credentials = new NetworkCredential("john1996doe1996@gmail.com", "saodzpcotcmhunho");
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;

                MailMessage mail = new MailMessage();
                mail.To.Add(personnel.Email);
                mail.From = new MailAddress("john1996doe1996@gmail.com", "Yeni Şifre");
                mail.IsBodyHtml = true;
                mail.Subject = "Yeni Şifre";

                mail.Body += "Merhaba Sayın " + personnel.Name + " " + personnel.Surname + "<br/> Kullanici Adiniz = " + personnel.Email + "<br/> Sifreniz: " + newPassword + "<br/>Giriş Yapmak için tıklayınız:" + "https://aspnetcoreappweb20221113160658.azurewebsites.net/";

                client.Send(mail);


                var result = await userManager.CreateAsync(personnel, newPassword);


                if (result.Succeeded)
                {
                    //personnel.PasswordHash = passwordHasher.HashPassword(personnel, userSignUp.Personnel.PasswordHash);
                    //IdentityResult Iresult = await userManager.UpdateAsync(personnel);
                    IdentityResult rslt = await userManager.AddToRoleAsync(personnel, Role);
                    return RedirectToAction("Index", "Manager");


                }
                else
                {
                    ViewBag.Message = "Personel eklenemedi";
                    userSignUp.Departments = departmentService.GetListAll();
                    return View(userSignUp);
                }


            }
            else
            {
                ViewBag.Message = "Personel eklenemedi";
                userSignUp.Departments = departmentService.GetListAll();
                return View(userSignUp);
            }
        }
        //////////////     ADVANCE
        public IActionResult GetListAdvance()
        {
            List<Advance> advances = advanceService.GetListAll();
            return View(advances);
        }

        public IActionResult DetailsAdvance(int id)
        {
            Advance advance = advanceService.GetById(id);
            return View(advance);
        }

        [HttpPost]
        public IActionResult ApproveAdvance(int id)
        {
            Advance advance = advanceService.GetById(id);
            advance.Approval = CoreLayer.Enums.Approval.Onaylandı;
            advance.ApprovalDate = DateTime.Now;
            var result = advanceService.Update(advance);
            return RedirectToAction("GetListAdvance");
        }

        public IActionResult RefusalAdvanceApprow(int id)
        {
            Advance advance = advanceService.GetById(id);
            return View(advance);
        }
        [HttpPost]
        public IActionResult RefusalAdvancetApprow(Advance advanceVM, int id)
        {
            Advance advance = advanceService.GetById(id);
            advance.ManagerDescription = advanceVM.ManagerDescription;
            var result = advanceService.Update(advance);
            return View(advance);
        }

        [HttpPost]
        public IActionResult RefusalAdvance(Advance advanceVM,int id)
        {
            Advance advance = advanceService.GetById(id);
            advance.Approval = CoreLayer.Enums.Approval.Reddedildi;
            advance.ApprovalDate = DateTime.Now;
            advance.ManagerDescription = advanceVM.ManagerDescription;
            var result = advanceService.Update(advance);
            return RedirectToAction("GetListAdvance");
        }



        //////////////     PERMIT
        public IActionResult GetListPermit()
        {
            List<Permit> permit = permitService.GetListAll();
            return View(permit);
        }

        public IActionResult DetailsPermit(int id)
        {
            Permit permit = permitService.GetById(id);
            return View(permit);
        }

        [HttpPost]
        public IActionResult ApprovePermit(int id)
        {
            Permit permit = permitService.GetById(id);
            permit.Approval = CoreLayer.Enums.Approval.Onaylandı;
            permit.ApprovalDate = DateTime.Now;
            var result = permitService.Update(permit);
            return RedirectToAction("GetListPermit");
        }

        public IActionResult RefusalPermitApprow(int id)
        {
            Permit permit = permitService.GetById(id);
            return View(permit);
        }

        [HttpPost]
        public IActionResult RefusalPermitApprow(Permit permitVM, int id)
        {
            Permit permit = permitService.GetById(id);
            permit.ManagerDescription = permitVM.ManagerDescription;
            var result = permitService.Update(permit);
            return View(permit);
        }

        [HttpPost]
        public IActionResult RefusalPermit(Permit permitVM, int id)
        {
            Permit permit = permitService.GetById(id);
            permit.Approval = CoreLayer.Enums.Approval.Reddedildi;
            permit.ApprovalDate = DateTime.Now;
            permit.ManagerDescription = permitVM.ManagerDescription;
            var result = permitService.Update(permit);
            return RedirectToAction("GetListPermit");
        }


        //////////////     EXPENSE
        public IActionResult GetListExpense()
        {

            List<Expense> expenses = expenseService.GetListAll();
            return View(expenses);
        }

        public IActionResult DetailsExpense(int id)
        {
            Expense expense = expenseService.GetById(id);
            return View(expense);
        }

        [HttpPost]
        public IActionResult ApproveExpense(Expense expenseVM, int id)
        {
            Expense expense = expenseService.GetById(id);
            expense.Approval = CoreLayer.Enums.Approval.Onaylandı;
            expense.ApprovalDate = DateTime.Now;
            var result = expenseService.Update(expense);
            return RedirectToAction("GetListExpense");
        }

        public IActionResult RefusalExpenseApprow(int id)
        {
            Expense expense = expenseService.GetById(id);
            return View(expense);
        }
        [HttpPost]
        public IActionResult RefusalExpenseApprow(Expense expenseVM, int id)
        {
            Expense expense = expenseService.GetById(id);
            expense.ManagerDescription = expenseVM.ManagerDescription;
            var result = expenseService.Update(expense);
            return View(expense);
        }

        [HttpPost]
        public IActionResult RefusalExpense(Expense expenseVM, int id)
        {
            Expense expense = expenseService.GetById(id);
            expense.Approval = CoreLayer.Enums.Approval.Reddedildi;
            expense.ApprovalDate = DateTime.Now;
            expense.ManagerDescription = expenseVM.ManagerDescription;
            var result = expenseService.Update(expense);
            return RedirectToAction("GetListExpense");
        }


    }
}
