using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly DBContext _context;
        private readonly ICache _cache;
        private readonly IMapper _mapper;

        public ClientsController(DBContext context, ICache cache, IMapper mapper)
        {
            _context = context;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var clients = _cache.GetAllClients();
            if(clients == null)
            {
                clients = _mapper.Map<IEnumerable<ClientResource>>(await _context.Clients.ToListAsync());
                _cache.SetAllClients(clients);
            }
            return View(clients);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _cache.GetClient(id.Value);
            if(client == null)
            {
                client = await _context.Clients
                    .Include(x => x.Subscriptions).ThenInclude(y => y.Company)
                    .Include(x => x.Subscriptions).ThenInclude(y => y.Client)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (client == null)
                {
                    return NotFound();
                }
            }

            return View(client);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,PESEL,BirthDate")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                _cache.InvalidateClient(client.Id);
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _cache.GetClient(id.Value);
            if (client == null)
            {
                client = await _context.Clients
                    .Include(x => x.Subscriptions).ThenInclude(y => y.Company)
                    .Include(x => x.Subscriptions).ThenInclude(y => y.Client)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (client == null)
                {
                    return NotFound();
                }
                _cache.AddOrUpdateClient(client);
            }

            var companies = _cache.GetAllCompanies();
            if (companies == null)
            {
                companies = _mapper.Map<IEnumerable<CompanyResource>>(await _context.Companies.ToListAsync());
                _cache.SetAllCompanies(companies);
            }
            var subs = companies.Where(x => client.Subscriptions.All(y => y.CompanyId != x.Id)).AsEnumerable();

            ViewBag.SelectList = new SelectList(subs, "Id", "Name");
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,PESEL,BirthDate")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _cache.InvalidateClient(client.Id);
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _cache.GetClient(id.Value);
            if (client == null)
            {
                client = await _context.Clients
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (client == null)
                {
                    return NotFound();
                }
            }

            return View(client);
        }

        public async Task<IActionResult> DeleteSubscribtion(int? clientId, int? companyId)
        {
            if (clientId == null || companyId == null)
            {
                return NotFound();
            }

            var client = _cache.GetClient(clientId.Value);
            if (client == null)
            {
                client = await _context.Clients
                    .Include(x => x.Subscriptions).ThenInclude(y => y.Company)
                    .Include(x => x.Subscriptions).ThenInclude(y => y.Client)
                    .FirstOrDefaultAsync(m => m.Id == clientId.Value);
                if (client == null)
                {
                    return NotFound();
                }
            }

            var companyData = client.Subscriptions.FirstOrDefault(x => x.CompanyId == companyId.Value);
            if (companyData == null)
            {
                return NotFound();
            }

            _cache.InvalidateClient(clientId.Value);
            var sub = await _context.ClientCompany.FirstOrDefaultAsync(x => x.ClientId == clientId && x.CompanyId == companyId);
            _context.ClientCompany.Remove(sub);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = clientId.Value });
        }

        public async Task<IActionResult> AddSubscribtion(int? clientId, int? companyId)
        {
            if (clientId == null || companyId == null)
            {
                return NotFound();
            }

            var client = _cache.GetClient(clientId.Value);
            if (client == null)
            {
                client = await _context.Clients
                    .Include(x => x.Subscriptions).ThenInclude(y => y.Company)
                    .Include(x => x.Subscriptions).ThenInclude(y => y.Client)
                    .FirstOrDefaultAsync(m => m.Id == clientId.Value);
                if (client == null)
                {
                    return NotFound();
                }
            }

            var companyData = client.Subscriptions.FirstOrDefault(x => x.CompanyId == companyId.Value);
            if (companyData != null)
            {
                return BadRequest();
            }

            _cache.InvalidateClient(clientId.Value);
            var sub = new ClientCompany()
            {
                ClientId = clientId.Value,
                CompanyId = companyId.Value
            };
            _context.ClientCompany.Add(sub);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = clientId.Value });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _cache.InvalidateClient(id);
            var client = await _context.Clients.FindAsync(id);
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _cache.GetClient(id) != null;
        }
    }
}
