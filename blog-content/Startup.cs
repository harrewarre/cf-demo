using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blog_content.Config;
using blog_content.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace blog_content
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        private readonly ILogger<Startup> _logger;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCloudFoundryOptions(Configuration);

            services.AddOptions();

            services.AddSingleton(provider =>
            {
                var cloudServiceConfig = Configuration.GetSection("vcap").Get<CloudFoundryServicesOptions>();
                var storageConnectionString = cloudServiceConfig.Services["user-provided"].First(s => s.Name == "content-storage").Credentials["connectionString"].Value;

                var storageConfig = new StorageConfig();
                storageConfig.ConnectionString = storageConnectionString;

                return Options.Create(storageConfig);
            });

            services.AddTransient(typeof(IStorageService), typeof(StorageService));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            _logger.LogInformation("Services configured!");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            _logger.LogInformation("Application configured!");
        }
    }
}
