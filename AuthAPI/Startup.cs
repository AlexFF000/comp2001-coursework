using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;
using System;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AuthAPI.Models;
using AuthAPI.Controllers.api;

namespace AuthAPI
{
    public class Startup
    {
        public static string PrivateKey;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddXmlDataContractSerializerFormatters();
            services.AddDbContext<COMP2001_ARedmondContext>(options => options.UseSqlServer(Configuration.GetConnectionString("socem1")));
            services.AddControllers();
            // Fetch private key used for signing tokens from appsettings
            PrivateKey = Configuration.GetValue<string>("PrivateKey");
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Only validate the key and expiry time of the token, as tokens will not be given any other fields
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateActor = false,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(PrivateKey))
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Do not hide IdentityModel error info
                IdentityModelEventSource.ShowPII = true;
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
