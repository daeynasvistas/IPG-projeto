using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Alfa_1.Models
{
    public class Profile // ADD Profile ---- 0.2
    {        
        public int Id { get; set; }
        [Display(Name = "Nome")]
        public string DisplayName { get; set; }
        [Display(Name = "Data de registo")]
        public DateTime RegisterDate { get; set; }
        [Display(Name = "Foto")]
        public string ProfilePicture { get; set; }

        //Navigation entre profile<->applicationUser
        public ApplicationUser User { get; set; }



    }
}
