using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BankAtmMVC.Data;
using BankAtmMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;


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

        [BindProperty]
        public InputModel Input { get; set; }



        //GET: Transaction/Deposit
        public IActionResult Deposit()
        {
            return View();
        }

        //POST: Transaction/Deposit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit([Bind("Amount")]Transaction transaction)
        {
            var currentId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            transaction.BankUserID = currentId;
            transaction.Date = DateTime.Now;
            transaction.Type = TransactionType.Deposit;
            var bankUser = _context.AspNetUsers.Where(i => i.Id == currentId).First();
            
            try{

                if (ModelState.IsValid)
                {
                  bankUser.Balance += Input.Amount; 
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

        //GET: Transaction/Withdraw
        public IActionResult Withdraw()
        {
            return View();
        }

        //POST: Transaction/Withdraw
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw([Bind("Amount")]Transaction transaction)
        {
            var currentId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            transaction.BankUserID = currentId;
            transaction.Date = DateTime.Now;
            transaction.Type = TransactionType.Withdraw;
            var bankUser = _context.AspNetUsers.Where(i => i.Id == currentId).First();

            try
            {

                if (ModelState.IsValid && (bankUser.Balance-Input.Amount)>0)
                {
                    bankUser.Balance -= Input.Amount;
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

        public async Task<IActionResult> PersonalTransactions()
        {
            var currentId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var userTransactions = await _context.AspNetUsers
                                .Include(s => s.Transactions)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(m => m.Id == currentId);



            return View(userTransactions);
        }

        [Route("{id}")]
        public async Task<IActionResult> PersonalTransactions(string id)
        {
            var currentId = id;
           
            var userTransactions = await _context.AspNetUsers
                                .Include(s => s.Transactions)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(m => m.Id == currentId);



            return View(userTransactions);
        } 

        public class InputModel : Transaction { }

        }
    }

    

    