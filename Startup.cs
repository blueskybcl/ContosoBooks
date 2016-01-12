using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using ContosoBooks.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.Http;
using ContosoBooks.Middleware;

namespace ContosoBooks
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services
            services.AddMvc();
            services.AddEntityFramework()
               .AddSqlServer()
               .AddDbContext<BookContext>(options => options.UseSqlServer(Configuration["Data:ConnectionString"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            SampleData.Initialize(app.ApplicationServices);
            ConfigureLogInline(app, loggerFactory);
            //ConfigureMapping(app);
            //ConfigureMapWhen(app);
            //ConfigureLogMiddleware(app, loggerFactory);
        }
        public void ConfigureLogMiddleware(IApplicationBuilder app,ILoggerFactory loggerfactory)
        {
            loggerfactory.AddConsole(minLevel: LogLevel.Information);

            app.UseRequestLogger();

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello from Middleware" );
            });
        }
        public void ConfigureLogInline(IApplicationBuilder app, ILoggerFactory loggerfactory)
        {
            loggerfactory.AddConsole(minLevel: LogLevel.Information);
            var logger = loggerfactory.CreateLogger("LogInline");
            app.Use(async (context, next) =>
            {
                logger.LogInformation("Handling request.");
                await next.Invoke();
                logger.LogInformation("Finished handling request.");
            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello from LogInline");
            });
        }
        public void ConfigureEnvironmentOne(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello from EnvironmentOne");
            });
        }
        public void ConfigureEnvironmentTwo(IApplicationBuilder app)
        {
            app.Use(next => async context =>
            {
                await context.Response.WriteAsync("Hello from EnvironmentTwo");
            });
        }
        private static void HandleMapTest(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Map Test Successful");
            });
        }
        public void ConfigureMapping(IApplicationBuilder app)
        {
            app.Map("/maptest", HandleMapTest);
        }
        private static void HandleBranch(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Branch used.");
            });
        }
        public void ConfigureMapWhen(IApplicationBuilder app)
        {
            app.MapWhen(context => {
                return context.Request.Query.ContainsKey("branch");
            }, HandleBranch);

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello from MapWhen");
            });
        }
        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
