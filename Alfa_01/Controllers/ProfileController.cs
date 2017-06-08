using Alfa_1.Data;
using Alfa_1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Alfa_1.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _environment;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment environment) 
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }


        // GET: Profile

        public async Task<IActionResult> Index()
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);
            var profile = await _context.Profile
                .SingleOrDefaultAsync(m => m.Id == currentUser.ProfileId); 
            return View(profile);
        }

        // GET: Profile/Edit/5
        public async Task<IActionResult> Edit() 
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);
            var profile = await _context.Profile.SingleOrDefaultAsync(m => m.Id == currentUser.ProfileId);

            return View(profile);
        }

        // POST: Profile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,DisplayName,ProfilePicture,ProfilePictureFile")] Profile profile, IFormFile ProfilePictureFile)
        {

            if (ModelState.IsValid)
            {
                ApplicationUser currentUser = await _userManager.GetUserAsync(User);

                if(ProfilePictureFile != null)
                {
                    string uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
                    Directory.CreateDirectory(Path.Combine(uploadPath, currentUser.Id));
                    string filename = Path.GetFileName(ProfilePictureFile.FileName);

                        using (FileStream fs = new FileStream(Path.Combine(uploadPath, currentUser.Id,"profile.png"), FileMode.Create))
                        {
                            await ProfilePictureFile.CopyToAsync(fs);
                        }

                    profile.ProfilePicture = "Profile.jpg";
                }

                _context.Update(profile);
                await _context.SaveChangesAsync();
            }
            return View(profile);
        }



    }
}
