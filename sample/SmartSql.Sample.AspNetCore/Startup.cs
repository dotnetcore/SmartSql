using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace SmartSql.Sample.AspNetCore
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services
                .AddSmartSql()
                .AddRepositoryFromAssembly(o =>
                {
                    o.AssemblyString = "SmartSql.Sample.AspNetCore";
                    o.Filter = (type) =>
                    {
                        return type.Namespace == "SmartSql.Sample.AspNetCore.DyRepositories";
                    };
                });
            RegisterConfigureSwagger(services);
        }
        private void RegisterConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "SmartSql.Sample.AspNetCore",
                    Version = "v1",
                    Contact = new Contact
                    {
                        Url = "https://github.com/Smart-Kit/SmartSql",
                        Email = "ahoowang@qq.com",
                        Name = "SmartSql"
                    },
                    Description = "SmartSql.Sample.AspNetCore"
                });
                c.CustomSchemaIds((type) => type.FullName);
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseStaticFiles();
            app.UseSwagger(c =>
            {

            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartSql.Sample.AspNetCore");
            });
        }
    }
}
