using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bell.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Bell.Controllers;
using Bell.Models;

using Bell.Models.Books;
using Bell.Models.Loans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;



namespace Bell
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // GDPR - General Data Protection Regulation
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.ConsentCookie.HttpOnly = true;
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<IBookRepository, BookRepository>();
            services.AddTransient<ILoanRepository, LoanRepository>();
            services.Add(new ServiceDescriptor(typeof(Loan), new LoanWithBookDetails()));
            /* add more transient services here as required */

            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            // services.AddMvc().AddViewLocalization().AddDataAnnotationsLocalization().AddRazorOptions(options =>
            // {
            //      options.ViewLocationExpanders.Remove(options.ViewLocationExpanders.First(f => f is Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure.PageViewLocationExpander));
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCookiePolicy();

            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseAuthentication();

            CreateRoles(serviceProvider).Wait();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "MainPage",
                    template: "{Controller=Home}/{action=Index}"
                );
            });
        }


        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            // setup Roles  
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = {"Admin", "User"};
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                // generate roles in the db if they don't exist
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // set up an Admin user, if not already in the db
            ApplicationUser user = await userManager.FindByEmailAsync("**************************");
            if (user == null)
            {
                user = new ApplicationUser()
                {
                    FirstName = "Admin",
                    LastName = "Admin",
                    UserName = "Admin",
                    Email = "**************************",
                    Address1 = "Rue de Ypres 21",
                    Address2 = "19",
                    CityRegion = "Brussels",
                    Country = "Belgium",
                    PostalCode = "1000",
                    PhoneNumber = "0012345678"
                };
                user.SecurityStamp = Guid.NewGuid().ToString();
                var result = await userManager.CreateAsync(user, "**************************");
                
            }
            await userManager.AddToRoleAsync(user, "Admin");

            // set up a standard user, if not already in the db
            ApplicationUser user1 = await userManager.FindByEmailAsync("**************************");
            if (user1 == null)
            {
                user1 = new ApplicationUser()
                {
                    FirstName = "user1",
                    LastName = "user1",
                    UserName = "user1",
                    Email = "**************************",
                    Address1 = "Rue de Ypres 21",
                    Address2 = "19",
                    CityRegion = "Brussels",
                    Country = "Belgium",
                    PostalCode = "1000",
                    PhoneNumber = "0012345678"
                };
                user.SecurityStamp = Guid.NewGuid().ToString();
                var result = await userManager.CreateAsync(user1, "**************************");
                
            }
            await userManager.AddToRoleAsync(user1, "User");
        }
    }
}
