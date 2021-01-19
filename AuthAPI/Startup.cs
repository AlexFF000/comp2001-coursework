using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;
using System.Text;
using Microsoft.EntityFrameworkCore;
using AuthAPI.Models;

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
            services.AddDbContext<DataAccess>();
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
