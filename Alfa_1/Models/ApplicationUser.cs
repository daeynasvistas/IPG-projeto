using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alfa_1.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        // ADD Profile ---- 0.2
        [ForeignKey("Profile")] // especifica a FK (ProfileId = FK em ApplicationUser )
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }

    }
}
