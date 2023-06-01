using FluentValidation.AspNetCore;
using HtmlTags;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using Topsis.Adapters.Algorithm;
using Topsis.Adapters.Caching;
using Topsis.Adapters.Database;
using Topsis.Adapters.Email;
using Topsis.Adapters.Encryption;
using Topsis.Adapters.Import;
using Topsis.Application;
using Topsis.Application.Contracts.Identity;
using Topsis.Domain.Contracts;
using Topsis.Web.Hubs;
using Topsis.Web.Infrastructure;
using Topsis.Web.Infrastructure.Tags;
using Topsis.Web.Services;

namespace Topsis.Web
{
    public class Startup
    {
        public const string RequireAdminPolicy = "RequireAdminPolicy";
        public const string RequireModeratorPolicy = "RequireModeratorPolicy";
        public const string RequireGuestPolicy = "RequireGuestPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCachingServices();
            services.AddMiniProfiler(opt =>
            {
                opt.RouteBasePath = "/profiler";
                opt.ResultsAuthorize = IsMonitorAuthorized;
            })
            .AddEntityFramework();

            services.AddDataAccess(Configuration);
            services.AddEmail(Configuration);
            services.AddAlgorithm();
            services.AddImportServices();

            services.AddDataProtectionToDatabase();
            services.Configure<CookiePolicyOptions>(options => {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                
            });

            services
                .AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<WorkspaceDbContext>();

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(15);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddApplicationServices();

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy(RequireAdminPolicy, policy => policy.RequireRole(RoleNames.Admin));
                opt.AddPolicy(RequireModeratorPolicy, policy => policy.RequireRole(RoleNames.Moderator));
                opt.AddPolicy(RequireGuestPolicy, policy => policy.RequireRole(RoleNames.Stakeholder));
            });

            services.AddHtmlTags(new TagConventions());

            services.AddRazorPages(opt =>
            {
                opt.Conventions.ConfigureFilter(new DbContextTransactionPageFilter());
                opt.Conventions.ConfigureFilter(new ValidatorPageFilter());

                opt.Conventions.AuthorizeAreaFolder("Admin", "/", RequireAdminPolicy);
                opt.Conventions.AuthorizeAreaFolder("Moderator", "/", RequireModeratorPolicy);
                opt.Conventions.AuthorizeAreaFolder("Guest", "/", RequireGuestPolicy);

                opt.Conventions.AllowAnonymousToAreaPage("Guest", "/Register");
            })
            .AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining<MessageBus>(); })
            .AddViewLocalization(
                LanguageViewLocationExpanderFormat.Suffix,
                opts => { opts.ResourcesPath = "Resources"; })
            .AddDataAnnotationsLocalization();

            services.AddSignalR();
            services.AddSignalRNotificationService();

            services.AddTransient<IEmailSender, EmailSender>();

            AddLocalization(services);

            services
                .AddMvc(opt => opt.ModelBinderProviders.Insert(0, new EntityModelBinderProvider()));
        }

        private bool IsMonitorAuthorized(HttpRequest request)
        {
            return request.HttpContext.User.IsInRole(RoleNames.Admin);
        }

        private static void AddLocalization(IServiceCollection services)
        {
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-GB"),
                        new CultureInfo("en-US"),
                        new CultureInfo("es"),
                        new CultureInfo("fr"),
                        new CultureInfo("en")
                    };

                    opts.DefaultRequestCulture = new RequestCulture("en-GB");
                    // Formatting numbers, dates, etc.
                    opts.SupportedCultures = supportedCultures;
                    // UI strings that we have localized.
                    opts.SupportedUICultures = supportedCultures;
                });

            services.AddSingleton<CommonLocalizationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiniProfiler();

            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            //var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<VotingHub>(VotingHub.RouteUrl);
            });

        }
    }
}
