using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class Cache : ICache
    {
        public Cache()
        {
            this._clientSubscribtionsSets = new Dictionary<int, IEnumerable<ClientResource>>();
            this._clientSet = new Dictionary<int, Client>();
        }

        private Dictionary<int, IEnumerable<ClientResource>> _clientSubscribtionsSets;
        private Dictionary<int, Client> _clientSet;
        private IEnumerable<ClientResource> _setOfAllClients;
        private IEnumerable<CompanyResource> _setOfAllCompanies;

        public IEnumerable<ClientResource> GetClientSubscribtionsSet(int companyId)
        {
            if (this._clientSubscribtionsSets.TryGetValue(companyId, out IEnumerable<ClientResource> result))
            {
                return result;
            }
            return null;
        }

        public IEnumerable<CompanyResource> GetAllCompanies()
        {
            if (this._setOfAllCompanies != null)
            {
                return this._setOfAllCompanies;
            }
            return null;
        }

        public void SetAllCompanies(IEnumerable<CompanyResource> set)
        {
            this._setOfAllCompanies = set;
        }

        public IEnumerable<ClientResource> GetAllClients()
        {
            if (this._setOfAllClients != null)
            {
                return this._setOfAllClients;
            }
            return null;
        }

        public void SetAllClients(IEnumerable<ClientResource> set)
        {
            this._setOfAllClients = set;
        }

        public void AddOrUpdateClient(Client client)
        {
            this._clientSet.Add(client.Id, client);
            foreach (var sub in client.Subscriptions)
            {
                if (this._clientSet.ContainsKey(client.Id))
                {
                    this._clientSet.Remove(client.Id);
                }
                this._clientSet.Add(client.Id, client);
            }
        }

        public Client GetClient(int id)
        {
            if(this._clientSet.TryGetValue(id, out Client result))
            {
                return result;
            }
            return null;
        }

        public void AddClientSubscribtion(int companyId, IEnumerable<ClientResource> clients)
        {
            this._clientSubscribtionsSets.Add(companyId, clients);
        }

        public void InvalidateClient(int id)
        {
            if (this._clientSet.TryGetValue(id, out Client clientData))
            {
                foreach (var company in clientData.Subscriptions)
                {
                    this._clientSubscribtionsSets.Remove(company.CompanyId);
                }

                this._clientSet.Remove(id);
            }
            this._setOfAllClients = null;
        }
    }
}
