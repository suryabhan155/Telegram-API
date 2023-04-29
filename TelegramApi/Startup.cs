using AutoMapper;
using BLL.Service.Abstract;
using BLL.Service.Implemention;
using DAL.Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using TelegramApi.AutoMapperConfigurations;
using AutoMapper;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Features;
using BOL;

namespace TelegramApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }         
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddDbContext<GeneralContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("DAL")));
            services.AddSingleton<TelegramService>();
            services.AddSingleton<ResponseModel>();
            services.AddTransient<IChannelService, ChannelService>();
            services.AddHostedService(provider => provider.GetService<TelegramService>());

            //add cookies policies
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //AutoMapper setup
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //mapping vm to model class
            services.AddSingleton(provider => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new Channel_Schema_Profiles());
            }).CreateMapper());

            //add api controllers
            services.AddControllers().AddJsonOptions(x =>
            {
                // serialize enums as strings in api responses (e.g. Role)
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

                // ignore omitted parameters on models to enable optional params (e.g. User update)
                x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            //large amount of json form data to controller
            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.HandleExceptionByJarvisAlgo(Configuration);
            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }

            app.UseCookiePolicy();
            
            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
        }
    }
}
