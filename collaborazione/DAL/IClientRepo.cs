using collaborazione.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.DAL
{
    public interface IClientRepo
    {
        Task<Client> AddUpdateClient(Client ClientChanges);
        Task<List<Progress>> GetProgressItems();
        Task<Progress> GetProgressItems(int id);
        Task<Client> GetClient(int id);
        IQueryable<Client> LoadClients(int pageIndex, int pageSize, string userId);
        IQueryable<Client> GetClients(string userId);
        Task<bool> DeleteUserAndItsClients(ApplicationUser user);
    }
}
