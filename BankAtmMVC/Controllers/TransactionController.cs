using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BankAtmMVC.Data;
using BankAtmMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BankAtmMVC.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;


        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: Transaction/Deposit
        public IActionResult Deposit()
        {
            return View();
        }

        //GET: Transaction/Deposit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit([Bind("Amount,Date")]Transaction transaction)
        {
            var currentId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            transaction.BankUserID = currentId;
            try{

                if (ModelState.IsValid)
                {

                  _context.Transactions.Add(transaction);
                  await _context.SaveChangesAsync();
                  return RedirectToAction("Index", "Home");
                }
            }
            catch (DbUpdateException)
            {
             ModelState.AddModelError("", "Unable to complete the transaction. " +
             "Try again, and if the problem persists " +
            "see your system administrator.");
            }

             return View(transaction);
        }
    }
}