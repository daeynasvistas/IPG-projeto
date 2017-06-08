using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Alfa_1.Data;
using Alfa_1.Models;
using Microsoft.AspNetCore.Identity;

namespace Alfa_1.Controllers
{
    public class ReportsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public ReportsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }


        // GET: Reports
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Report.Include(r => r.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Reports/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Report
                .Include(r => r.Category)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // GET: Reports/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name");
            ViewData["Created"] = DateTime.Now;
            //ViewData["Created"] = await _manager.GetUserAsync(HttpContext.User);

            return View();
        }

        // POST: Reports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Img,IsComplete,Latitude,Longitude,CategoryId")] Report report)
        {
            if (ModelState.IsValid)
            {
                report.User = await _userManager.GetUserAsync(HttpContext.User);
                report.Created = DateTime.Now;

                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name", report.CategoryId);
            return View(report);
        }

        // GET: Reports/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Report.SingleOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name", report.CategoryId);
            return View(report);
        }

        // POST: Reports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Description,Img,IsComplete,Created,Close,Latitude,Longitude,CategoryId")] Report report)
        {
            if (id != report.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // report complete check unchecked
                    if (report.IsComplete)
                    { report.Close = DateTime.Now; }
                    else { report.Close = default(DateTime); }

                    _context.Update(report);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportExists(report.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }



            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name", report.CategoryId);
            return View(report);
        }

        // GET: Reports/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Report
                .Include(r => r.Category)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // POST: Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var report = await _context.Report.SingleOrDefaultAsync(m => m.Id == id);
            _context.Report.Remove(report);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ReportExists(long id)
        {
            return _context.Report.Any(e => e.Id == id);
        }
    }
}
