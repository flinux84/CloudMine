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
using CloudMineServer.Data;
using CloudMineServer.Models;
using CloudMineServer.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CloudMineServer.Interface;
using CloudMineServer.Classes;
using CloudMineServer.Middleware.TokenProvider;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using CloudMineServer.API_server.Middleware.TokenProvider;
using System.Security.Claims;
using System.Security.Principal;

namespace CloudMineServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
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

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<ICloudMineDbService, CloudMineDbService>();

            services.AddDbContext<CloudDbRepository>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                    builder => builder.AllowAnyOrigin());
            });

            services.AddSignalR(options => options.Hubs.EnableDetailedErrors = true);

            services.AddMvc();
            //API VERSIONING
            services.AddApiVersioning(
            options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion( new DateTime( 2016, 11, 28 ) );
            } );
            //API PAGING
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            #region tokenProvider
            var secretKey = "mysupersecret_secretkey!123";
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "CloudMine",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "ExampleAudience",

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                LoginPath = new Microsoft.AspNetCore.Http.PathString("/loginForAdmin.html"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                AuthenticationScheme = "Cookie",
                CookieName = "access_token",
                TicketDataFormat = new CustomJwtDataFormat(
                    SecurityAlgorithms.HmacSha256, tokenValidationParameters)
            });
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });
            //var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var options = new TokenProviderOptions
            {
                Audience = "ExampleAudience",
                Issuer = "CloudMine",
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            };

            // Add JWT generation endpoint:
            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options));
            #endregion


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

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseCors("AllowAnyOrigin");
            app.UseStaticFiles();
            app.UseIdentity();
            app.UseWebSockets();
            app.UseSignalR();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "restAction",
                    template: "api/{controller}/{action}/");
            });
        }
    }
}
