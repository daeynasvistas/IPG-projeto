using Alfa_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alfa_1.Data
{
    public class SampleData
    {
        public static class DbInitializer
        {
            public static void Initialize(ApplicationDbContext context)
            {
                //context.Database.EnsureCreated();

                // Look for any category.
                if (context.Category.Any())
                {
                    return;   // DB has been seeded
                }

                var categories = new Category[]
                {
                new Category { Name = "Ambiente e espaçoes verdes",
                              Description = "Abate, plantação, substituição, poda de árvore" +
                                            "Danos por queda de árvores"+
                                            "Danos/manutenção de cercas, vedações e outras estruturas"+
                                            "Danos/manutenção de erva ou relva"+
                                            "Limpeza de espaços verdes (papéis, embalagens, etc.)"+
                                            "Reclamações sobre ruído"+
                                            "Recolha de aparas e ramos resultantes da poda de jardins particulares"+
                                            "Reparação de sistema de rega"},
               new Category { Name ="Higiene e limpeza urbana",
                              Description = "Denúncia de pragas e doenças (associadas a animais: pombos, ratos, etc.)" +
                                            "Entupimento de sarjetas ou sumidouros"+
                                            "Limpeza da via pública (varredura, lavagem)"+
                                            "Limpeza de espaços verdes (papéis, embalagens, entre outros)"+
                                            "Limpeza de graffities"+
                                            "Manutenção de instalações sanitárias, lavadouros ou balneários"+
                                            "Manutenção/reparação de mesas, bancos, bebedouros públicos, sarjetas e outro mobiliário urbano danificado"+
                                            "Pedido de contentores, sacos e fitas para resíduos urbanos" },

                };

                foreach (Category c in categories)
                {
                    context.Category.Add(c);
                }
                context.SaveChanges();
            }
        }
    }
}

