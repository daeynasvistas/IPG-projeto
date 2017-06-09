using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Alfa_1.Models
{
    public class Report
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Img { get; set; }
        // primeira fase.. completo/em aberto
        public bool IsComplete { get; set; }

        [Required]
        public DateTime Created { get; set; }
        public DateTime Close { get; set; }

        // EF core ainda não tem datatype para geospatial
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }

        //Navigation entre Report<->applicationUser
        public virtual ApplicationUser User { get; set; }


        [ForeignKey("Category")] // especifica a FK (CategoryId = FK em Report )
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
