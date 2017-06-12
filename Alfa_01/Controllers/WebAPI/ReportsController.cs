using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alfa_1.Data;
using Alfa_1.Models;
using Alfa_1.Models.ApiViewModel;

namespace Alfa_1.Controllers.WebAPI
{
    [Produces("application/json")]
    [Route("api/Reports")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context) 
        {
            _context = context;
        }


        // GET: api/Reports
        [HttpGet]
        public IEnumerable<AllReportsViewModel> Get()
        {
            // to hold list of Reports e outros detalhes
            List<AllReportsViewModel> ReportVMlist = new List<AllReportsViewModel>();
            // context com categoria e user
            var reports =  _context.Report.Include(c => c.Category)
                                          .Include(u => u.User)
                                          .Include(p => p.User.Profile); 
            // enviar informação pretendoda para a view
            foreach (Report item in reports)
            {
                AllReportsViewModel reportVM = new AllReportsViewModel(); // ViewModel
                // Report
                reportVM.Id         = item.Id;
                reportVM.Name       = item.Name;
                reportVM.Latitude   = item.Latitude;
                reportVM.Longitude  = item.Longitude;
                reportVM.Img        = item.Img;
                reportVM.Created    = item.Created;
                reportVM.Close      = item.Close;
                reportVM.IsComplete = item.IsComplete;
                // category
                reportVM.Category    = item.Category.Name;
                reportVM.CategoryId  = item.Category.CategoryId;
                // user
                reportVM.UserPicture = item.User.Profile.ProfilePicture;
                reportVM.DisplayName = item.User.Profile.DisplayName;

                ReportVMlist.Add(reportVM);
            }
            // envia já em JSon
            return ReportVMlist;                                    // return _context.Report;
        }

        // GET: api/Reports/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var report = await _context.Report.SingleOrDefaultAsync(m => m.Id == id);

            if (report == null)
            {
                return NotFound();
            }

            return Ok(report);
        }

        // PUT: api/Reports/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReport([FromRoute] long id, [FromBody] Report report)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != report.Id)
            {
                return BadRequest();
            }

            _context.Entry(report).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Reports
        [HttpPost]
        public async Task<IActionResult> PostReport([FromBody] Report report)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Report.Add(report);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReport", new { id = report.Id }, report);
        }

        // DELETE: api/Reports/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var report = await _context.Report.SingleOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            _context.Report.Remove(report);
            await _context.SaveChangesAsync();

            return Ok(report);
        }

        private bool ReportExists(long id)
        {
            return _context.Report.Any(e => e.Id == id);
        }
    }
}