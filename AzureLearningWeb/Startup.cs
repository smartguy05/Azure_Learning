using AzureLearning.Models;
using AzureLearningWeb.Security;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace AzureLearningWeb
{
    public class Startup
    {
        private readonly AppSettings _appSettings;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _appSettings = new AppSettings();
            Configuration.Bind(_appSettings);
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("AzurePolicy");

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_appSettings);
            services.AddHttpContextAccessor();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiKey", policy =>
                {
                    policy.Requirements.Add(new ApiKeyRequirement(_appSettings));
                });
            });

            services.AddTransient<IAuthorizationHandler, ApiKeyHandler>();

            services
                .AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddJwtBearer("AzureAD", options =>
                {
                    options.Audience = "ActiveDirectory:Audience";
                    options.Authority = "ActiveDirectory:Instance";
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = "ActiveDirectory:Instance",
                        ValidAudience = "ActiveDirectory:Audience"
                    };
                });

            // .AddAzureADBearer(options => Configuration.Bind("AzureActiveDirectory", options));

            services.AddCors(options =>
            {
                options.AddPolicy("AzurePolicy", builder => builder
                    .WithOrigins("https://localhost:44325/", "Access-Control-Allow-Origin",
                        "Access-Control-Allow-Credentials")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                );
            });

            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }
    }
}