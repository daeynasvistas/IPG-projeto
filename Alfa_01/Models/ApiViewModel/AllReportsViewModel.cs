using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alfa_1.Models.ApiViewModel
{
    public class AllReportsViewModel
    {
        // Report propriamente dito-------------------
        public string Name { get; set; }
        public long Id { get; set; }
        public string Img { get; set; }
        // primeira fase.. completo/em aberto
        public bool IsComplete { get; set; }
        public DateTime Created { get; set; }
        public DateTime Close { get; set; }
        // EF core ainda não tem datatype para geospatial
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }

        // Categoria do report-------------------
        public string Category { get; set; }
        public int CategoryId { get; set; }

        // Utilizador que enviou report-------------------
        public string DisplayName { get; set; }
        public string UserPicture { get; set; }

    }
}
