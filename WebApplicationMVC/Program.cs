using System.Net.Http.Headers;
using ClassLibraryDATA.Models;
using ClassLibraryREPO;
using ClassLibrarySERVICES;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpClient("WEB_API", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7224/api/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(60);
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = new System.Net.CookieContainer()
                };
            });
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddLogging(logging =>
            {
                logging.ClearProviders(); // Xóa các nhà cung cấp mặc định (tùy chọn)
                logging.AddConsole(); // Ghi log vào console
                logging.AddDebug(); // Ghi log vào cửa sổ Debug của IDE
                                    // logging.AddEventSourceLogger(); // (Tùy chọn) Ghi log vào Event Source
            });
            builder.Services.AddDbContext<FoodDeliverContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));

            builder.Services.AddScoped<IFoodRepositories, FoodRepositories>();
            builder.Services.AddScoped<IFoodServices, FoodServices>();

            builder.Services.AddScoped<IUserRepositories, UserRepositories>();
            builder.Services.AddScoped<IUserServices, UserServices>();


            builder.Services.AddScoped<IAIRecommendationService, AIRecommendationService>();

            builder.Services.AddScoped<ICategoryRepositories, CategoryRepositories>();
            builder.Services.AddScoped<ICategoryServices, CategoryServices>();

            builder.Services.AddScoped<IOrderServices, OrderServices>();
            builder.Services.AddScoped<IOrderRepositories, OrderRepositories>();

            builder.Services.AddScoped<IOrderDetailRepositories, OrderDetailRepositories>();
            builder.Services.AddScoped<IOrderDetailServices, OrderDetailServices>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSession();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
