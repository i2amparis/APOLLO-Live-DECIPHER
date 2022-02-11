using FluentValidation.AspNetCore;
using HtmlTags;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Globalization;
using Topsis.Adapters.Algorithm;
using Topsis.Adapters.Caching;
using Topsis.Adapters.Database;
using Topsis.Adapters.Email;
using Topsis.Adapters.Encryption;
using Topsis.Application;
using Topsis.Application.Contracts.Identity;
using Topsis.Domain.Contracts;
using Topsis.Web.Infrastructure;
using Topsis.Web.Infrastructure.Tags;
using Topsis.Web.Services;

namespace Topsis.Web
{
    public class Startup
    {
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
            })
            .AddEntityFramework();

            services.AddDataAccess(Configuration);
            services.AddEmail(Configuration);
            services.AddAlgorithm();

            services
                .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<WorkspaceDbContext>();

            services.AddApplicationServices();

            services.AddDataProtection(Configuration);
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy(RequireModeratorPolicy, policy => policy.RequireRole(RoleNames.Moderator));
                opt.AddPolicy(RequireGuestPolicy, policy => policy.RequireRole(RoleNames.Stakeholder));
            });

            services.AddHtmlTags(new TagConventions());

            services.AddRazorPages(opt =>
            {
                opt.Conventions.ConfigureFilter(new DbContextTransactionPageFilter());
                opt.Conventions.ConfigureFilter(new ValidatorPageFilter());

                opt.Conventions.AuthorizeAreaFolder("Moderator", "/", RequireModeratorPolicy);
                opt.Conventions.AuthorizeAreaFolder("Guest", "/", RequireGuestPolicy);

                opt.Conventions.AllowAnonymousToAreaPage("Guest", "/Register");
            })
            .AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining<MessageBus>(); })
            .AddViewLocalization(
                LanguageViewLocationExpanderFormat.Suffix,
                opts => { opts.ResourcesPath = "Resources"; })
            .AddDataAnnotationsLocalization();

            services.AddTransient<IEmailSender, EmailSender>();

            AddLocalization(services);

            services
                .AddMvc(opt => opt.ModelBinderProviders.Insert(0, new EntityModelBinderProvider()));
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
            });
        }
    }
}
