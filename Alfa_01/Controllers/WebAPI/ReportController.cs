using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alfa_1.Data;
using Alfa_1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Alfa_1.Controllers.WebAPI
{
    [Produces("application/json")]
    [Route("api/report")]
    public class ReportController : Controller
    {
        //private readonly SignInManager<ApplicationUser> _signInManager;
        //public ReportController(SignInManager<ApplicationUser> signInManager)
        //                        {
        //                            _signInManager = signInManager;
        //                        }


        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Report
        [HttpGet]
        public IEnumerable<Report> GetReport()
        {
            return _context.Report;
        }

        // GET: api/Report/5
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

        // PUT: api/Report/5
        [Authorize]
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

        // POST: api/Report
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostReport([FromBody] Report report)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

              var User = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Email == report.UserId);
              report.UserId = User.Id;
            //var info = await _signInManager.GetExternalLoginInfoAsync();

            report.Created = DateTime.Now;
            report.IsComplete = false;

            _context.Report.Add(report);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReport", new { id = report.Id }, report);
        }

        //// DELETE: api/Report/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteReport([FromRoute] long id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var report = await _context.Report.SingleOrDefaultAsync(m => m.Id == id);
        //    if (report == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Report.Remove(report);
        //    await _context.SaveChangesAsync();

        //    return Ok(report);
        //}

        private bool ReportExists(long id)
        {
            return _context.Report.Any(e => e.Id == id);
        }
    }
}