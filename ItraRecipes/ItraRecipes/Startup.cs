using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ItraRecipes.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;
using ItraRecipes.Services;

namespace ItraRecipes
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<UserModel, IdentityRole>(config =>
            {
                config.Password.RequireDigit = false;
                config.Password.RequiredLength = 6;
                config.Password.RequiredUniqueChars = 1;
                config.Password.RequireLowercase = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
                config.SignIn.RequireConfirmedEmail = true;
                config.User.RequireUniqueEmail = true;
            })
               .AddDefaultTokenProviders()
               .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication()
               .AddTwitter(twitterOptions =>
               {
                   twitterOptions.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
                   twitterOptions.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
               })
               .AddFacebook(facebookOptions =>
               {
                   facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                   facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
               })
               .AddVKontakte(o =>
               {
                   o.ClientId = "6809628";
                   o.ClientSecret = "ig7ThoJc5xOEPxfSlUax";
                   o.SaveTokens = true;
               });
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
