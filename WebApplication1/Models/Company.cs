using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NIP { get; set; }
        public string Industry { get; set; }

        public ICollection<ClientCompany> Clients { get; set; }
    }
}