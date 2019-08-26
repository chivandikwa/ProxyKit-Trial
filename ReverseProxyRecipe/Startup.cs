using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ProxyKit;

namespace ReverseProxyRecipe
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ProxyHandler>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddProxy(builder =>
            {
                builder.ConfigurePrimaryHttpMessageHandler(HttpClientExtensions.CreatePrimaryHandler);
                builder.ConfigureHttpClient(client => client.AddMediaTypeWithQualityHeaderValue());
            });
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseProxyForwarding<ProxyHandler>("/proxy-this");
        }
    }
}
