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
        public static string PrivateKey = @"MIICXgIBAAKBgQCcKH795t691X/OE53Vr+jl4jtrR8ynrbX8FXKoOoprMuGQ20uH
W8/BRnrNSNHyyz3iYWXAh54ePkIZe6DZTyYW++PFNX+qFv9nOCtCwc3QbZvXNvFr
y1H06xx0sfM5EX84SYEeIs3fTasrbd27kljUULWI5szh7WGCJbASF6MxYQIDAQAB
AoGBAI3aJ4njiCEv6To9DZqIgmsZOFq6zUjgfrkwjogNLaI1hTv+STz7hN0Qdgs3
BCimwV98lF7xkwvsCEV3zWS6BmFZv/aL85FIF0Pv2TYA0FlYd3pQtO0hyPgww+Ff
Vn8GXZxdrjSS32W0/BJPJ69N+hOD2GVSxJ7VvBsTqnwWg9w1AkEA5erX+6LJ7w3S
ZHv6XYSN4dhjfzvu901aZfr+3ezg5zBdxA6C8ZCzrgfMSjXXEl0+5k9OWoH18AXe
efOLN7i5NwJBAK3fkmCekwEkJYNZUt3kezSQ6vJaaDkin3mVpAhBOpkeqtyTF7g6
m/0h92SqUfc06TRAnVwyJWqdt05mSect1icCQQCwpEhCBWTdrCHbn8DP/po/TM/M
nhBPofd06GzNtkNoe6leisOhsknJyCX1Uf4mFsvPiFirgrOgL/IogauRKPPrAkEA
j6sUwFFHFsJPXOEJ399TDnRQLkEVrWA2dbh3+XoseSkl9Wx1RPzot5jBWqSBZcqK
h2p8nBmzYQ1A0b8jgaoWMQJAPHxNEyky16ubU38cEDtnOGJHD7gNNIhwnsfnvKaQ
oa2YYSv2/gsdSiDBbiSy0wWgPMPLHweOBdmDLvVLQsL9xw==";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddXmlDataContractSerializerFormatters();
            services.AddDbContext<COMP2001_ARedmondContext>();
            services.AddControllers();
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
