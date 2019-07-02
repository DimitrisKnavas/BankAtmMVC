using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankAtmMVC.Data;
using BankAtmMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankAtmMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministratorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdministratorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchValue)
        {
            var users = from m in _context.AspNetUsers select m;

            if (!String.IsNullOrEmpty(searchValue))
            {
                users = users.Where(s => s.LastName.Contains(searchValue) 
                                      || s.FirstName.Contains(searchValue)
                                      || s.Email.Contains(searchValue)
                                      || s.Id.Contains(searchValue));
            }
            
            return View(await users.ToListAsync());
        }
    }
}