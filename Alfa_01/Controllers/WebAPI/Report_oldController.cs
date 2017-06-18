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
    [Route("api/Report_old")]
    public class Report_oldController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Report_oldController(ApplicationDbContext context) 
        {
            _context = context;
        }


        // GET: api/Report
        [HttpGet]
        public IEnumerable<ReportViewModel> Getall()  
        {
            // to hold list of Reports e outros detalhes
            List<ReportViewModel> ReportVMlist = new List<ReportViewModel>();
            // context com categoria e user
            var reports =  _context.Report.Include(c => c.Category)
                                          .Include(u => u.User)
                                          .Include(p => p.User.Profile); 
            // enviar informação pretendoda para a view
            foreach (Report item in reports)
            {
                ReportViewModel reportVM = new ReportViewModel(); // ViewModel
                // Report
                reportVM.Id         = item.Id;
                reportVM.Name       = item.Name;
                reportVM.Latitude   = item.Latitude;
                reportVM.Longitude  = item.Longitude;
             //   reportVM.Img        = item.Img;
                reportVM.Created    = item.Created;
                reportVM.Close      = item.Close;
                reportVM.IsComplete = item.IsComplete;
                // category
          //      reportVM.Category    = item.Category.Name;
                reportVM.CategoryId  = item.Category.CategoryId;
                // user
                reportVM.UserPicture = item.User.Profile.ProfilePicture;
                reportVM.DisplayName = item.User.Profile.DisplayName;

                ReportVMlist.Add(reportVM);
            }
            // envia já em JSon
            return ReportVMlist;                                    // return _context.Report;
        }

        // GET: api/Report/5
        [HttpGet("{id}", Name = "GetReport")]
        public async Task<IActionResult> GetReport([FromRoute] long id)
        {
      //      if (!ModelState.IsValid)
      //      {
      //          return BadRequest(ModelState);
      //      }
      //      // to hold list of Reports e outros detalhes
      //      List<ReportViewModel> ReportVMlist = new List<ReportViewModel>();

      //      var report = await _context.Report.Include(c => c.Category)
      //                                         .Include(u => u.User)
      //                                         .Include(p => p.User.Profile)
      //                                          .SingleOrDefaultAsync(m => m.Id == id);

      //      ReportViewModel reportVM = new ReportViewModel(); // ViewModel
      //                                                                // Report
      //      reportVM.Id = report.Id;
      //      reportVM.Name = report.Name;
      //      reportVM.Latitude = report.Latitude;
      //      reportVM.Longitude = report.Longitude;
      //      reportVM.Img = report.Img;
      //      reportVM.Created = report.Created;
      //      reportVM.Close = report.Close;
      //      reportVM.IsComplete = report.IsComplete;
      //      // category
      // //     reportVM.Category = report.Category.Name;
      //      reportVM.CategoryId = report.Category.CategoryId;
      //      // user
      ////      reportVM.UserPicture = report.User.Profile.ProfilePicture;
      ////      reportVM.DisplayName = report.User.Profile.DisplayName;

      //      ReportVMlist.Add(reportVM);


      //      if (report == null)
      //      {
      //          return NotFound();
      //      }
            var reportVM = await _context.Report.SingleOrDefaultAsync(u => u.Id == id);
            return Ok(reportVM);
        }

        // PUT: api/Report/5
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
        [HttpPost]
        public async Task<IActionResult> PostReport([FromBody] Report report) // alterei de Report para o ReportViewModel
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Report _report = new Report();

            //_report.Latitude = report.Latitude;
            //_report.Longitude = report.Longitude;
            //_report.Name = report.Name;
            //_report.Img = report.Img;
            //_report.CategoryId = report.CategoryId;
            //_report.Created = DateTime.Now;
            //_report.User = await _context.ApplicationUser.SingleOrDefaultAsync(u => u.Id == report.UserId);
            //_context.Report.Add(_report);
            _report = report;
            _report.Category = await _context.Category.SingleOrDefaultAsync(u => u.CategoryId == report.CategoryId);
            _report.User = await _context.ApplicationUser.SingleOrDefaultAsync(u => u.Id == report.UserId);

            _context.Report.Add(_report);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReport", new { controller = "Report", id = report.Id });

        }

        // DELETE: api/Report/5
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