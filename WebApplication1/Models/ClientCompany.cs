using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class ClientCompany
    {
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
