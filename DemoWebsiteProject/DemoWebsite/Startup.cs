using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DemoWebsite.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;

namespace DemoWebsite
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<FormattingService>();

            services.AddTransient<FeatureToggles>(x => new FeatureToggles
            {
                EnableDeveloperExceptions = Configuration.GetValue<bool>("FeatureToggles:EnableDeveloperExceptions")
            });

            services.Configure<SmtpConfig>(Configuration.GetSection("Smtp"));

            services.AddTransient<FilesLibrary>(x => new FilesLibrary
            {
                Url = Configuration.GetValue<string>("FilesLibrary:Url")
            });

            services.AddDbContext<EmployeeDataContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString("EmployeeDataContext");
                options.UseSqlServer(connectionString);
            });

            services.AddDbContext<FilesDataContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString("FilesDataContext");
                options.UseSqlServer(connectionString);
            });

            services.AddDbContext<IdentityDataContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString("IdentityDataContext");
                options.UseSqlServer(connectionString);
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDataContext>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env,
            FeatureToggles features,
            ILoggerFactory loggerFactory
            )
        {           
            if (features.EnableDeveloperExceptions)
            {
                //app.UseDeveloperExceptionPage();
            }

            app.UseIdentity();

            app.UseMvc(routes =>
            {
                routes.MapRoute("Default",
                    "{controller=Home}/{action=Index}/{id?}"
                );
            });

            app.UseFileServer();

            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            location = location.Substring(0, location.LastIndexOf(Path.DirectorySeparatorChar));
            location = location + Path.DirectorySeparatorChar + "NLog.config";

            env.ConfigureNLog(location);
            loggerFactory.AddNLog();
            app.AddNLogWeb();

            app.UseExceptionHandler("/Home/Error");
            app.UseStatusCodePagesWithRedirects("/StatusCode/{0}");

            app.UseStaticFiles();
        }
    }
}
