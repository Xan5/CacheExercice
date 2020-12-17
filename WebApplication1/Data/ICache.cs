using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public interface ICache
    {
        void AddClientSubscribtion(int companyId, IEnumerable<ClientResource> clients);
        void AddOrUpdateClient(Client client);
        Client GetClient(int id);
        IEnumerable<ClientResource> GetAllClients();
        IEnumerable<CompanyResource> GetAllCompanies();
        IEnumerable<ClientResource> GetClientSubscribtionsSet(int companyId);
        void InvalidateClient(int id);
        void SetAllClients(IEnumerable<ClientResource> set);
        void SetAllCompanies(IEnumerable<CompanyResource> set);
    }
}