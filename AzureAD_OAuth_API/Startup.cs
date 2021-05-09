using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAD_OAuth_API
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AzureAD_OAuth_API", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri("https://login.microsoftonline.com/60879ec9-9cda-42a6-9611-4bc2ee2fcedc/oauth2/v2.0/authorize"),
                            TokenUrl = new Uri("https://login.microsoftonline.com/60879ec9-9cda-42a6-9611-4bc2ee2fcedc/oauth2/v2.0/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "api://1642dc16-891c-4f3c-9769-6c7f38e2a989/ReadWriteAccess", "Reads the Weather forecast" }
                            }
                        }
                    }
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                     {
                     new OpenApiSecurityScheme
                        {
                        Reference = new OpenApiReference
                        {
                        Type = ReferenceType.SecurityScheme,
                        Id = "oauth2"
                        },
                                Scheme = "oauth2",
                                Name = "oauth2",
                                In = ParameterLocation.Header
                     },
                        new List<string>()
                     }
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                   c.SwaggerEndpoint("/swagger/v1/swagger.json", "AzureAD_OAuth_API v1");
                   //c.RoutePrefix = string.Empty;
                   c.OAuthClientId("1642dc16-891c-4f3c-9769-6c7f38e2a989");
                   c.OAuthClientSecret("Mp5YKhbwbd4hUh3dDk9Ca.61RTd_Sv~IT~");
                   c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
                }
                );
            }

            app.UseHttpsRedirection();

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
