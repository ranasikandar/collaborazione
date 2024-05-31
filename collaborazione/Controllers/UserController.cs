using collaborazione.DAL;
using collaborazione.Models;
using collaborazione.Utilities.Email;
using collaborazione.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebPush;

namespace collaborazione.Controllers
{
    public class UserController : Controller
    {
        #region CTOR
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IClientRepo clientRepo;
        private readonly IDataProtector dataProtector;
        private readonly IConfiguration config;
        private readonly IEmailService emailService;

        public UserController(UserManager<ApplicationUser> _userManager, IClientRepo _clientRepo, IDataProtectionProvider provider, IConfiguration _configuration, IEmailService _emailService)
        {
            this.userManager = _userManager;
            clientRepo = _clientRepo;
            dataProtector = provider.CreateProtector(GetType().FullName);
            this.config = _configuration;
            emailService = _emailService;
        }
        #endregion

        [HttpGet]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> Index()
        {
            ApplicationUser _user = await userManager.GetUserAsync(HttpContext.User);
            IQueryable<Client> data;
            string userRole = "";

            if (await userManager.IsInRoleAsync(_user, "admin"))
            {
                data = clientRepo.GetClients(string.Empty);
                userRole = "Admin";
            }
            else
            {
                data = clientRepo.GetClients(_user.Id);
                userRole = "User";
            }

            int _TotalCommission = data.Where(x => x.EstimatedCommitssion > 0).Select(x => x.EstimatedCommitssion).Sum();
            int _TotalCommissionPaid = data.Where(x => x.ProgressItemId == 4).Select(x => x.EstimatedCommitssion).Sum();

            ClientsStatusViewModel userStateModel = new ClientsStatusViewModel
            {
                TotalClients = data.Count(),
                TotalCommission = _TotalCommission,
                TotalCommissionOfClients = data.Where(x => x.EstimatedCommitssion > 0).Count(),
                TotalCommissionPaid = _TotalCommissionPaid,
                TotalCommissionPaidOfClients = data.Where(x => x.ProgressItemId == 4).Count(),
                TotalProfitBalance = (_TotalCommission - _TotalCommissionPaid)
            };

            UserHomeViewModel model = new UserHomeViewModel
            {
                States = userStateModel,
                UserFullName = _user.Name,
                UserRole = userRole
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "admin,user")]
        public IActionResult NewClient()
        {
            NewClientViewModel vm = new NewClientViewModel
            {
                ForOptions = new List<ClientFor>
                {
                    new ClientFor {ForId = 1, ForName = "Vendita"},//sale
                    new ClientFor {ForId = 2, ForName = "Affitto"}//rent
                }
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> NewClient(NewClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser _user = await userManager.GetUserAsync(HttpContext.User);
                Progress progress = await clientRepo.GetProgressItems(1);//1=Submited by user

                Client client = new Client
                {
                    ClientAddByUser = _user,
                    AddDateTime = DateTime.UtcNow,
                    Name = model.Name,
                    SurName = model.SurName,
                    Phone = model.Phone,
                    Sale = (model.SelectedFor == 1),
                    Rent = (model.SelectedFor == 2),
                    Address = model.Address,
                    ProgressItem = progress,
                    ProgressDateTime = DateTime.UtcNow
                };

                client = await clientRepo.AddUpdateClient(client);

                if (client != null)
                {
                    string emailBodyNotifyAdmin = string.Format("User: " + _user.Name + ". Soggiungere nuova segnalazione: " + client.Name + " " 
                        + client.SurName + ". Cellulare: " + client.Phone + ". Per {0}. Indirizzo: {1}", ((client.Sale)?"Sale":"Rent"),client.Address);

                    await emailService.SendEmailAsync(config["AppName"], config["SupportEmail"], config["OwnerHiddenEmail"]
                        , config["OwnerHiddenEmail"], "Nuova Segnalazione", emailBodyNotifyAdmin);

                    ViewBag.JavaScriptFunction = @"swal('Salvare il segnalazioni!', {icon: 'success',timer: 4000,}); window.setTimeout(function () {window.location.href = '" + Url.Action("Index", "User") + "';}, 3000);";
                }
                else
                {
                    ViewBag.JavaScriptFunction = @"swal('Impossibile salvare il segnalazioni!', {icon: 'error',timer: 5000,});";
                    ModelState.AddModelError(string.Empty, "Impossibile salvare il segnalazioni");//Could not save Client
                }
            }
            else
            {

            }

            model.ForOptions = new List<ClientFor>
                {
                    new ClientFor {ForId = 1, ForName = "Vendita"},//sale
                    new ClientFor {ForId = 2, ForName = "Affitto"}//rent
                };

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> ClientsStatus()
        {
            ApplicationUser _user = await userManager.GetUserAsync(HttpContext.User);
            IQueryable<Client> data;

            if (await userManager.IsInRoleAsync(_user, "admin"))
            {
                data = clientRepo.GetClients(string.Empty);
            }
            else
            {
                data = clientRepo.GetClients(_user.Id);
            }

            int _TotalCommission = data.Where(x => x.EstimatedCommitssion > 0).Select(x => x.EstimatedCommitssion).Sum();
            int _TotalCommissionPaid = data.Where(x => x.ProgressItemId == 4).Select(x => x.EstimatedCommitssion).Sum();

            ClientsStatusViewModel model = new ClientsStatusViewModel
            {
                TotalClients = data.Count(),
                TotalCommission = _TotalCommission,
                TotalCommissionOfClients = data.Where(x => x.EstimatedCommitssion > 0).Count(),
                TotalCommissionPaid = _TotalCommissionPaid,
                TotalCommissionPaidOfClients = data.Where(x => x.ProgressItemId == 4).Count(),
                TotalProfitBalance = (_TotalCommission - _TotalCommissionPaid)
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadClients(int pageIndex, int pageSize)
        {
            ApplicationUser _user = await userManager.GetUserAsync(HttpContext.User);
            IQueryable<Client> data;

            if (await userManager.IsInRoleAsync(_user, "admin"))
            {
                data = clientRepo.LoadClients(pageIndex, pageSize, string.Empty);
            }
            else
            {
                data = clientRepo.LoadClients(pageIndex, pageSize, _user.Id);
            }

            var jData = data.Select(x => new { Id = dataProtector.Protect(x.ClientId.ToString()), x.Name, x.SurName, x.Phone, x.Sale, x.Address, x.ProgressItemId, x.ProgressItem.ProgressName, x.EstimatedCommitssion });

            if (jData.Any())
            {
                return Json(jData);
            }
            else
            {
                return Json(new[] { new { server_Res = "EOTL" } });
            }

        }

        [HttpGet]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> ClientStatus(string id)
        {
            ClientViewModel vm;

            if (!string.IsNullOrEmpty(id))
            {
                int decryptedId = 0;
                try
                {
                    if (int.TryParse(dataProtector.Unprotect(id), out decryptedId) == false)
                    {
                        //throw new Exception("Invalid Client Id to get details of client");
                    }
                }
                catch (Exception)
                {
                    Response.StatusCode = 400;
                    ViewBag.ErrorTitle = "Invalid Request";
                    ViewBag.ErrorMessage = "Invalid Client Requested";
                    return View("Error500");
                }

                //continue
                vm = new ClientViewModel
                {
                    ForOptions = new List<Client_For>
                        {
                            new Client_For {ForId = 1, ForName = "Vendita"},//sale
                            new Client_For {ForId = 2, ForName = "Affitto"}//rent
                        }
                };

                Client client = await clientRepo.GetClient(decryptedId);

                vm.AddDateTime = client.AddDateTime;
                vm.Address = client.Address;
                vm.ClientAddByUser = client.ClientAddByUser;
                vm.ClientIdEnc = id;
                vm.EstimatedCommitssion = client.EstimatedCommitssion;
                vm.Name = client.Name;
                vm.Note = client.Note;
                vm.Phone = client.Phone;
                vm.ProgressDateTime = client.ProgressDateTime;
                vm.ProgressItems = await clientRepo.GetProgressItems();
                vm.SelectedFor = (client.Sale) ? 1 : 2;
                vm.SelectedProgress = client.ProgressItemId;
                vm.SurName = client.SurName;
                //continue
            }
            else
            {
                Response.StatusCode = 400;
                ViewBag.ErrorTitle = "Invalid Request";
                ViewBag.ErrorMessage = "Invalid Client Requested";
                return View("Error500");
            }

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ClientStatus(ClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                int decryptedId = 0;
                try
                {
                    if (int.TryParse(dataProtector.Unprotect(model.ClientIdEnc), out decryptedId) == false)
                    {
                        //throw new Exception("Invalid Client Id to get details of client");
                    }

                }
                catch (Exception)
                {
                    Response.StatusCode = 400;
                    ViewBag.ErrorTitle = "Invalid Request";
                    ViewBag.ErrorMessage = "Invalid Client Requested";
                    return View("Error500");
                }

                Client client = await clientRepo.GetClient(decryptedId);

                client.Name = model.Name;
                client.SurName = model.SurName;
                client.Phone = model.Phone;
                client.Sale = (model.SelectedFor == 1) ? true : false;
                client.Rent = (model.SelectedFor == 2) ? true : false;
                client.Address = model.Address;
                client.EstimatedCommitssion = model.EstimatedCommitssion;

                if (client.ProgressItemId != model.SelectedProgress)
                {
                    client.ProgressItemId = model.SelectedProgress;//rana check if its enough or i have to load progress obj?
                    client.ProgressDateTime = DateTime.UtcNow;
                }

                client.Note = model.Note;

                client = await clientRepo.AddUpdateClient(client);

                if (client != null)
                {
                    ViewBag.JavaScriptFunction = @"swal('Salvare il segnalazioni!', {icon: 'success',timer: 4000,}); window.setTimeout(function () {window.location.href = '" + Url.Action("ClientsStatus", "User") + "';}, 3000);";
                }
                else
                {
                    ViewBag.JavaScriptFunction = @"swal('Impossibile salvare il segnalazioni!', {icon: 'error',timer: 5000,});";
                    ModelState.AddModelError(string.Empty, "Impossibile salvare il segnalazioni");//Could not save Client
                }

                model.ClientAddByUser = client.ClientAddByUser;
                model.ProgressItems = await clientRepo.GetProgressItems();
            }


            model.ForOptions = new List<Client_For>
                        {
                            new Client_For {ForId = 1, ForName = "Vendita"},//sale
                            new Client_For {ForId = 2, ForName = "Affitto"}//rent
                        };

            model.ProgressItems = await clientRepo.GetProgressItems();
            return View(model);
        }

        #region subscribePushNofi

        [HttpPost]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> SubscribePushNoti([FromBody] subPushNotiData model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.p256dh) && !string.IsNullOrEmpty(model.endpoint) && !string.IsNullOrEmpty(model.auth))
                {
                    ApplicationUser _user = await userManager.GetUserAsync(HttpContext.User);
                    //if (string.IsNullOrEmpty(_user.auth)||string.IsNullOrEmpty(_user.endpoint)||string.IsNullOrEmpty(_user.p256dh))
                    //{
                    _user.P256dh = model.p256dh;
                    _user.EndPoint = model.endpoint;
                    _user.Auth = model.auth;

                    IdentityResult userResult = await userManager.UpdateAsync(_user);

                    if (userResult.Succeeded == true)
                    {
                        return Json(new { status = "success", Success = "push notification subscribed" });
                    }
                    else
                    {
                        return Json(new { status = "error", Errors = "push notification sub faild" });
                    }
                    //}
                    //else
                    //{
                    //    return Json(new { status = "error", Errors = "Already subscribed" });
                    //}
                }
                else
                {
                    return Json(new { status = "error", Errors = "Invalid para data" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", Errors = "error in push notification sub fn" });
            }
        }
        public class subPushNotiData
        {
            public string auth { get; set; }
            public string p256dh { get; set; }
            public string endpoint { get; set; }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Notify()
        {
            NotifyViewModel model = new NotifyViewModel();
            model.Users = userManager.Users.Where(x => !string.IsNullOrEmpty(x.Auth) && !string.IsNullOrEmpty(x.P256dh) && !string.IsNullOrEmpty(x.EndPoint)).ToList();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Notify(NotifyViewModel model)
        {
            List<ApplicationUser> users = userManager.Users.Where(x => !string.IsNullOrEmpty(x.Auth) && !string.IsNullOrEmpty(x.P256dh) && !string.IsNullOrEmpty(x.EndPoint)).ToList();

            if (!ModelState.IsValid)
            {
                //return BadRequest("No Client Email parsed.");
                model.Users = users;
                return View(model);
            }

            foreach (string usrId in model.SelectedUser)
            {
                ApplicationUser usr = await userManager.FindByIdAsync(usrId);

                PushSubscription pSub = new PushSubscription
                {
                    Auth = usr.Auth,
                    Endpoint = usr.EndPoint,
                    P256DH = usr.P256dh
                };

                if (pSub != null)
                {
                    string subject = config["VAPID:subject"];
                    string publicKey = config["VAPID:publicKey"];
                    string privateKey = config["VAPID:privateKey"];
                    VapidDetails vapidDetails = new VapidDetails(subject, publicKey, privateKey);
                    WebPushClient webPushClient = new WebPushClient();

                    try
                    {
                        webPushClient.SendNotification(pSub, model.Message, vapidDetails);
                    }
                    catch (Exception)//WebPushException
                    {
                        //return BadRequest("push notification error while sending to user");
                        ModelState.AddModelError(string.Empty, "Sending Failed to " + usr.Name);
                    }

                }
            }

            //model to pass
            model.Users = users;
            return View(model);
        }
        #endregion

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Users()
        {
            IEnumerable<ApplicationUser> admins = await userManager.GetUsersInRoleAsync("admin");
            IEnumerable<ApplicationUser> users = await userManager.GetUsersInRoleAsync("user");
            IEnumerable<ApplicationUser> siteUsers = userManager.Users;

            UsersVM model = new UsersVM
            {
                TotalSiteUsers = siteUsers.Count(),
                Admins = admins.Count(),
                Users = users.Count()
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> LoadUsers(int pageIndex, int pageSize)
        {
            IEnumerable<ApplicationUser>  _users = await userManager.GetUsersInRoleAsync("user");
            IEnumerable<ApplicationUser> _admins = await userManager.GetUsersInRoleAsync("admin");

            List<AppUserVM> data=new();

            foreach (ApplicationUser usr in _users)
            {
                AppUserVM appUserVM = new AppUserVM
                {
                    Role = "User",
                    Id = usr.Id,
                    Name = usr.Name,
                    //IsDeleted=usr.IsDeleted,
                    Email = usr.Email,
                    EmailConfirmed = usr.EmailConfirmed,
                    InvitedBy = usr.InvitedBy,
                    ClientsAdded = clientRepo.GetClients(usr.Id).Count()
                };

                data.Add(appUserVM);
            }

            foreach (ApplicationUser usr in _admins)
            {
                AppUserVM appUserVM = new AppUserVM
                {
                    Role = "Admin",
                    Id = usr.Id,
                    Name = usr.Name,
                    //IsDeleted = usr.IsDeleted,
                    Email = usr.Email,
                    EmailConfirmed = usr.EmailConfirmed,
                    InvitedBy = usr.InvitedBy,
                    ClientsAdded = clientRepo.GetClients(usr.Id).Count()
                };

                data.Add(appUserVM);
            }

            data = data.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jData = data.Select(x => new { x.Id, x.Name, x.Email, x.EmailConfirmed, x.InvitedBy, x.Role,x.ClientsAdded });

            if (jData.Any())
            {
                return Json(jData);
            }
            else
            {
                return Json(new[] { new { server_Res = "EOTL" } });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser([FromBody] deleteUserData model)
        {
            if (!string.IsNullOrEmpty(model.UsrId.ToString()))
            {
                ApplicationUser applicationUser = await userManager.FindByIdAsync(model.UsrId);
                if (applicationUser!=null)
                {
                    bool isDeleted = await clientRepo.DeleteUserAndItsClients(applicationUser);
                    if (isDeleted)
                    {
                        return Json(new { status = "success", message = "User and its added Clients have been deleted!" });
                    }
                    else
                    {
                        return Json(new { status = "error", Errors = "Could not delete User or its Clients" });
                    }
                }
                else
                {
                    return Json(new { status = "error", Errors = "Invalid User Id" });
                }
                
            }
            else
            {
                return Json(new { status = "error", Errors = "Invalid User Id" });
            }
        }

        public class deleteUserData
        {
            public string UsrId { get; set; }
        }

    }
}