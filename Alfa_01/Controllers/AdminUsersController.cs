using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Alfa_1.Data;
using Alfa_1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Alfa_1.Controllers
{
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminUsersController(ApplicationDbContext context, 
                                    RoleManager<IdentityRole> roleManager,
                                    UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: AdminUsers
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["EmailSortParm"] = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            // context do user
            var users = from u in _context.ApplicationUser.Include(a => a.Profile)
                        select u;
            // search stuff
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.Email.Contains(searchString)
                                       || s.Profile.DisplayName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "email_desc":
                    users = users.OrderByDescending(s => s.Email);
                    break;
                case "Date":
                    users = users.OrderBy(s => s.Profile.RegisterDate);
                    break;
                case "date_desc":
                    users = users.OrderByDescending(s => s.Profile.RegisterDate);
                    break;
                default:
                    users = users.OrderBy(s => s.Email);
                    break;
            }

            int pageSize = 3;
            return View(await PaginatedList<ApplicationUser>.CreateAsync(users.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: AdminUsers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUser
                .Include(a => a.Profile)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

     
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {   
            var applicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Id == id);
            //ViewData["Roles"] = new SelectList(_roleManager.Roles.OrderBy(r => r.Name));
            List<string> roles = _roleManager.Roles.Select(x => x.Name).ToList();
            ViewData["Roles"] = roles;
            return View(applicationUser);
        }

        // POST: AdminUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        //        public async Task<IActionResult> Edit(string id, [Bind("ProfileId,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] ApplicationUser applicationUser)
        public async Task<IActionResult> EditPost(string id)
        {

            string role = Request.Form["NewRole"].ToString();
            var applicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(u => u.Id == id);

            if (await TryUpdateModelAsync<ApplicationUser>(
                applicationUser,
                "",
                u => u.EmailConfirmed, u => u.LockoutEnabled))
            {
                try
                    //------------------------- MELHORAR ISTO .. o user já está no Applicationuser estranho
                {   //var user = await _userManager.FindByIdAsync(id);
                    await _userManager.AddToRoleAsync(applicationUser, role);
                    //--------------------------------------------------------------------------------------
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException /* ex */)
                {   //primeira tentativa de !!!!!try catch!!!!

                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(applicationUser);
        }

        // GET: AdminUsers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUser
                .Include(a => a.Profile)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // POST: AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Id == id);
            _context.ApplicationUser.Remove(applicationUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.ApplicationUser.Any(e => e.Id == id);
        }
    }
}
