using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Alfa_1.Data;
using Alfa_1.Models;
using Alfa_1.Services;
using Microsoft.AspNetCore.Identity;
using Alfa_1.Configuration;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;

using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Alfa_1
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //---------------- DAEY add 401 em vez de redirect para login na api
            services.Configure<IdentityOptions>(config =>
            {
                config.Cookies.ApplicationCookie.Events =
                    new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = ctx =>
                        {
                            if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode != 200)
                            {
                                ctx.Response.StatusCode = 401;
                                return Task.FromResult<object>(null);
                            }

                            ctx.Response.Redirect(ctx.RedirectUri);
                            return Task.FromResult<object>(null);
                        }
                    };
            });



            // Envio de email
            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
            }) 
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();


            // Configure using a sub-section of the appsettings.json file.
            services.Configure<SmtpConfig>(Configuration.GetSection("Smtp"));
            services.Configure<AppConfiguration>(Configuration.GetSection("AppConfiguration"));

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("AppConfiguration:Key").Value)),
                    ValidAudience = Configuration.GetSection("AppConfiguration:SiteUrl").Value,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = Configuration.GetSection("AppConfiguration:SiteUrl").Value
                }
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // -------------- inicialiar o crateRoles DAEY
            // ADD ROLES ---- 0.1
            await CreateRoles(serviceProvider);
            // problemas com "." em latitude e longitude.. alterar cultureinfo
            System.Globalization.CultureInfo customCulture = new CultureInfo("PT");
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            CultureInfo.DefaultThreadCurrentCulture = customCulture;
        }


        /// Criar Roles no startup DAEY com user login pass no appsetting.json
        /// // ADD ROLES ---- 0.1
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //adding custom roles
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "Manager", "Member" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                //creating the roles and seeding them to the database
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            //creating a super user who could maintain the web app
            var poweruser = new ApplicationUser
            {
                UserName = Configuration.GetSection("AppSettings")["UserEmail"],
                Email = Configuration.GetSection("AppSettings")["UserEmail"],
                EmailConfirmed = true,// autoconfirmar o email enviado
                Profile = new Profile {
                                       RegisterDate = DateTime.Now,
                                       ProfilePicture = "Profile.png"
                                       } // // criar novo profile vazio para utilizador ADD Profile ---- 0.2
            };

            string UserPassword = Configuration.GetSection("AppSettings")["UserPassword"];
            var _user = await userManager.FindByEmailAsync(Configuration.GetSection("AppSettings")["UserEmail"]);

            if (_user == null)
            {
                var createPowerUser = await userManager.CreateAsync(poweruser, UserPassword);

                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the "Admin" role 
                    await userManager.AddToRoleAsync(poweruser, "Admin");
                }
            }
        }
    }
}
