using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BankAtmMVC.Models
{
    public class Transaction
    {
        public int ID { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }


        public string BankUserID { get; set; }
        public BankUser AspNetUsers { get; set; }
    }
}
