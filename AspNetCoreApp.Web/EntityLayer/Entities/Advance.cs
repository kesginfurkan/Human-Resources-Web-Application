using CoreLayer.Enums;
using MusteriOrnegiCoreWeb.Models.Custom_Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Entities
{
   
    public class Advance : BaseEntity
    {
        public int ID { get; set; }
        public bool Status { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Miktar girişi zorunludur.")]
        [SifirdanBuyukOlsun]
        public decimal AdvanceAmount { get; set; }
        public Approval Approval { get; set; } = Approval.OnayBekliyor;

        [Required(ErrorMessage = "Ücreti tipini belirtiniz.")]
        public Currency Currency { get; set; }

        [Required(ErrorMessage = "Açıklama giriniz.")]
        [MinLength(50, ErrorMessage= "Avans açıklaması minimum 50 karakter olmalıdır.")]
       
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime ApprovalDate { get; set; }

        
        public string PersonnelID { get; set; }
        public Personnel Personnel { get; set; }

        
    }
}
