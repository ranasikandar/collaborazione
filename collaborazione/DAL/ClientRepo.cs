using collaborazione.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.DAL
{
    public class ClientRepo : IClientRepo
    {
        #region ctor
        private readonly AppDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public ClientRepo(AppDbContext context, UserManager<ApplicationUser> _userManager)
        {
            this.context = context;
            this.userManager = _userManager;
        }
        #endregion

        public async Task<Client> AddUpdateClient(Client ClientChanges)
        {
            //chk wather update or add new
            if (ClientChanges.ClientId > 0)
            {
                var client = context.Clients.Attach(ClientChanges);
                client.State = EntityState.Modified;
            }
            else
            {
                context.Clients.Add(ClientChanges);
            }

            await context.SaveChangesAsync();
            return ClientChanges;
        }

        public async Task<List<Progress>> GetProgressItems()
        {
            return await context.Progreses.OrderBy(x => x.ProgressId).ToListAsync();
        }

        public async Task<Progress> GetProgressItems(int id)
        {
            return await context.Progreses.Where(x => x.ProgressId == id).FirstOrDefaultAsync();
        }

        public async Task<Client> GetClient(int id)
        {
            return await context.Clients.Where(x => x.ClientId == id).Include(x => x.ClientAddByUser).Include(x => x.ProgressItem).FirstOrDefaultAsync();
        }

        public IQueryable<Client> LoadClients(int pageIndex, int pageSize,string userId)
        {
            if (string.IsNullOrEmpty(userId))//is admin
            {
                return context.Clients.OrderByDescending(x => x.ClientId).Skip(pageIndex * pageSize).Take(pageSize).Include(x => x.ProgressItem);
            }
            else
            {
                return context.Clients.Include(x => x.ClientAddByUser).Where(x => x.ClientAddByUser.Id == userId).OrderByDescending(x => x.ClientId).Skip(pageIndex * pageSize).Take(pageSize).Include(x => x.ProgressItem);
            }
        }

        public IQueryable<Client> GetClients(string userId)
        {
            if (string.IsNullOrEmpty(userId))//is admin
            {
                return context.Clients;
            }
            else
            {
                return context.Clients.Where(x => x.ClientAddByUser.Id == userId);
            }
        }

        public async Task<bool> DeleteUserAndItsClients(ApplicationUser user)
        {
            try
            {
                IEnumerable<Client> clients = context.Clients.Where(x => x.ClientAddByUser == user);
                if (clients.Any())
                {
                    context.Clients.RemoveRange(clients);
                }

                IList<string> allUserRoles = await userManager.GetRolesAsync(user);
                IdentityResult roleResult = await userManager.RemoveFromRolesAsync(user, allUserRoles);
                if (roleResult.Succeeded)
                {
                    IdentityResult delUsrResult = await userManager.DeleteAsync(user);
                    if (delUsrResult.Succeeded)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {
                return false;
            }

        }

    }
}
