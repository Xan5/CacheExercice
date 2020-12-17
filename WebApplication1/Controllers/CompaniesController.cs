using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class CompaniesController : Controller
    {
        private readonly DBContext _context;
        private readonly ICache _cache;
        private readonly IMapper _mapper;

        public CompaniesController(DBContext context, ICache cache, IMapper mapper)
        {
            this._context = context;
            this._cache = cache;
            this._mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var companies = this._cache.GetAllCompanies();
            if (companies == null)
            {
                companies = this._mapper.Map<IEnumerable<CompanyResource>>(await _context.Companies.ToListAsync());
                this._cache.SetAllCompanies(companies);
            }
            return View(companies);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.Include(x => x.Clients).ThenInclude(x => x.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }
    }
}
