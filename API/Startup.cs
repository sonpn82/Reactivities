using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Application.Activities;
using Application.Core;
using API.Extensions;
using FluentValidation.AspNetCore;
using API.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using API.SignalR;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;  // create this field instead of MS default field

        public Startup(IConfiguration config)
        {
            _config = config;
            //Configuration = configuration; remove default field
        }

        // public IConfiguration Configuration { get; }  remove default prop

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opt => {
                // all API end points will require authentication
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
                .AddFluentValidation(config => {  // add FluentValidation to our service
                    config.RegisterValidatorsFromAssemblyContaining<Create>();  // where the valiation is - in Application.Activities.Create
                 });

            // Move all services to Extention/ApplicationServiceExtention
            // To keep the startup file small & clean
            services.AddApplicationServices(_config);
            // Add Authentication service
            services.AddIdentityServices(_config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // use our customized exception middle ware
            app.UseMiddleware<ExceptionMiddleware>();     

            // header security tightening using 3rd party addon installation: NWebsec.AspNetCore.Middleware
            // after pulish to heroku
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opt => opt.NoReferrer());
            app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
            app.UseXfo(opt => opt.Deny());
            app.UseCsp(opt => opt
                .BlockAllMixedContent()
                .StyleSources(s => s.Self().CustomSources
                    (
                        "https://cdn.jsdelivr.net",
                        "https://fonts.googleapis.com",
                        "sha256-yChqzBduCCi4o4xdbXRXh4U/t1rP4UUUMJt+rB+ylUI="
                    ))
                .FontSources(s => s.Self().CustomSources
                    (
                        "https://fonts.gstatic.com", 
                        "data:",
                        "https://googleapis.com",
                        "https://cdn.jsdelivr.net"
                    ))
                .FormActions(s => s.Self())
                .FrameAncestors(s => s.Self())
                .ImageSources(s => s.Self().CustomSources
                    (
                        "https://res.cloudinary.com", 
                        "https://www.facebook.com",
                        "https://platform-lookaside.fbsbx.com",
                        "data:"
                    ))
                .ScriptSources(s => s.Self().CustomSources
                    (
                        "https://www.facebook.com",
                        "https://connect.facebook.net",
                        "sha256-CVapyt2yaKPS6X25sJHXdTUvyYS+CaqPHqx/ty9xO6Y="
                    ))
            );

            // end of security settings

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();  remove this one
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIv5 v1"));
            }
            else {
                // this code is for production - after deploy to heroku
                // add more security
                app.Use(async (context, next) => 
                {
                    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");  // 315360000 = 1 year in secs
                    await next.Invoke();
                });
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            // for production - run the default index.html
            app.UseDefaultFiles();

            // for production - server static files to client - in wwwroot folder after run build (if not names root then must do additional config)
            app.UseStaticFiles();

            app.UseCors("CorsPolicy"); // to allow localhost:3000 to access API data
            // Authentication must be put before Authorization
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat");  // for adding and sending comments to all activity participant in real time
                endpoints.MapFallbackToController("Index", "Fallback"); // for production, fallback controller
            });
        }
    }
}
