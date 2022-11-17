using CoreLayer.Entities;
using CoreLayer.Enums;
using System;
using System.Collections.Generic;

namespace AspNetCoreApp.Web.Models
{
    public class FilterExpenseVM
    {
     
        public TypeOfExpenses TypeOfExpenses { get; set; }

   
        public List<Expense> Expenses { get; set; }


    }
}
