using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Alfa_1.Models;
using Alfa_1.Data;

namespace Alfa_1.Controllers.WebAPI
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }



        [HttpGet("")]
        public List<Profile> List()
        {
            // all -- passhash included
            List<Profile> allUsers = _context.Profile.ToList();

            return allUsers;

            //return new List<Profile>{
            //    new Profile{DisplayName = "Daniel"},
            //    new Profile{DisplayName = "IPGfix"}
            // };

        }
    }
}