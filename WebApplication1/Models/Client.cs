using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int PESEL { get; set; }
        public DateTime BirthDate { get; set; }

        public ICollection<ClientCompany> Subscriptions { get; set; }
    }
}
