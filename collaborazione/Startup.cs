using collaborazione.DAL;
using collaborazione.Models;
using collaborazione.Security;
using collaborazione.Utilities;
using collaborazione.Utilities.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using WebMarkupMin.AspNetCore5;

namespace collaborazione
{
    public class Startup
    {
        #region ctor
        private readonly IConfiguration Configuration;
        public IWebHostEnvironment _Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _Environment = environment;
        }
        #endregion


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //SHARED HOSTING LOSE KEYS, KEEP IN FILE SO USER DONT GETS LOGOUT
            // create a directory for keys if it doesn't exist
            // it'll be created in the root, beside the wwwroot directory
            var keysDirectoryName = "Keys";
            var keysDirectoryPath = Path.Combine(_Environment.ContentRootPath, keysDirectoryName);
            if (!Directory.Exists(keysDirectoryPath))
            {
                Directory.CreateDirectory(keysDirectoryPath);
            }
            services.AddDataProtection()
              .PersistKeysToFileSystem(new DirectoryInfo(keysDirectoryPath))
              .SetApplicationName("CustomCookieAuthentication");
            //SHARED HOSTING LOSE KEYS, KEEP IN FILE SO USER DONT GETS LOGOUT

            #region set ConsentCookie policy exp and many more options
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddYears(1)
                //, Domain="", HttpOnly=true, IsEssential=true, MaxAge= TimeSpan.FromDays(30.00), Secure=true
            };

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.ConsentCookie.Expiration = TimeSpan.FromDays(365);
                options.ConsentCookie.Name = "AgreeCookiePolicy";
            });
            #endregion

            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("ConnectionString")));

            #region identity options overide

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 3;
                //options.Password.RequiredUniqueChars = 3;

                options.SignIn.RequireConfirmedEmail = true;

                //options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

                options.Lockout.MaxFailedAccessAttempts = 100;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);

            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            //.AddTokenProvider<CustomEmailConfirmationTokenProvider
            //    <ApplicationUser>>("CustomEmailConfirmation");

            services.Configure<DataProtectionTokenProviderOptions>(o =>
                        o.TokenLifespan = TimeSpan.FromHours(24));

            services.Configure<CustomEmailConfirmationTokenProviderOptions>(o =>
                        o.TokenLifespan = TimeSpan.FromDays(30));

            #endregion

            // Add WebMarkupMin services for html minify. if (!_Environment.IsDevelopment())
            if (Convert.ToBoolean(Configuration["MinifyCompressHTML"].ToString()))
            {
                //services.AddWebMarkupMin()
                //.AddHtmlMinification()
                //.AddXmlMinification()
                //.AddHttpCompression()
                //;

                services.AddWebMarkupMin(options =>
                {
                    options.AllowMinificationInDevelopmentEnvironment = true;
                    options.AllowCompressionInDevelopmentEnvironment = true;
                    options.DisablePoweredByHttpHeaders = true;
                })
                .AddHtmlMinification(options =>
                {
                    //options.ExcludedPages = new List<IUrlMatcher>
                    //{
                    //    new WildcardUrlMatcher("/minifiers/x*ml-minifier"),
                    //    new ExactUrlMatcher("/contact")
                    //};

                    //HtmlMinificationSettings settings = options.MinificationSettings;
                    //settings.RemoveRedundantAttributes = true;
                    //settings.RemoveHttpProtocolFromAttributes = true;
                    //settings.RemoveHttpsProtocolFromAttributes = true;

                    //options.CssMinifierFactory = new NUglifyCssMinifierFactory();
                    //options.JsMinifierFactory = new NUglifyJsMinifierFactory();
                })
                .AddXhtmlMinification(options =>
                {
                    //options.IncludedPages = new List<IUrlMatcher>
                    //{
                    //    new WildcardUrlMatcher("/minifiers/x*ml-minifier"),
                    //    new ExactUrlMatcher("/contact")
                    //};

                    //XhtmlMinificationSettings settings = options.MinificationSettings;
                    //settings.RemoveRedundantAttributes = true;
                    //settings.RemoveHttpProtocolFromAttributes = true;
                    //settings.RemoveHttpsProtocolFromAttributes = true;

                    //options.CssMinifierFactory = new KristensenCssMinifierFactory();
                    //options.JsMinifierFactory = new CrockfordJsMinifierFactory();
                })
                .AddXmlMinification(options =>
                {
                    //XmlMinificationSettings settings = options.MinificationSettings;
                    //settings.CollapseTagsWithoutContent = true;
                })
                .AddHttpCompression(options =>
                {
                    //options.CompressorFactories = new List<ICompressorFactory>
                    //{
                    //    new BrotliCompressorFactory(new BrotliCompressionSettings
                    //    {
                    //        Level = 1
                    //    }),
                    //    new DeflateCompressorFactory(new DeflateCompressionSettings
                    //    {
                    //        Level = CompressionLevel.Fastest
                    //    }),
                    //    new GZipCompressorFactory(new GZipCompressionSettings
                    //    {
                    //        Level = CompressionLevel.Fastest
                    //    })
                    //};
                })
                ;
            }
            // Add WebMarkupMin services for html minify.

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            }).AddXmlSerializerFormatters();

            services.AddAuthentication()
                //.AddFacebook(options =>
                //{
                //    options.AppId = "594098507968017";
                //    options.AppSecret = "df41ef262f11415e8005352079e014dc";
                //})
                ;

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                options.LogoutPath = "/Logout";

                options.Cookie.HttpOnly = true;

                options.Cookie.Name = "UserIdentityCollab";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromDays(365);
            });

            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = "X_CSRF_TOKEN_COLLAB";
                options.FormFieldName = "CSRF_TOKEN_COLLAB_FORM5";
            });//doesnt work the ajax post back with anti forgre token NOW USING GLOBALY options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()

            services.Configure<CookieTempDataProviderOptions>(opt => opt.Cookie.Name = "CollabTempDataProvider");

            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IViewToStringService, ViewToStringService>();
            services.AddScoped<IClientRepo, ClientRepo>();
            services.AddSingleton<DataProtectionPurposeStrings>();
            services.AddTransient<Utilities.Helper>();

            //boiler plate code. for what?
            //services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                //TRY TO ENFORCE SSL The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();//REDIRICT TO HTTPS IF HTTP REQUESTED
            app.UseCookiePolicy();

            FileServerOptions fileServerOptions = new FileServerOptions();
            fileServerOptions.EnableDirectoryBrowsing = false;
            app.UseFileServer(fileServerOptions);

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            //html minify //if (!env.IsDevelopment())
            if (Convert.ToBoolean(Configuration["MinifyCompressHTML"].ToString()))
            {
                app.UseWebMarkupMin();
            }
            //html minify

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
