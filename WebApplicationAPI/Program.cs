using System.Net.Http.Headers;
using Azure;
using ClassLibraryDATA.Models;
using ClassLibraryREPO;
using ClassLibrarySERVICES;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using WebApplicationAPI.ChatHub;

namespace WebApplicationAPI
{
    public class Program
    {
        //"applicationUrl": "https://localhost:7224;http://localhost:5114",
        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Category>("Categories");
            builder.EntitySet<Food>("Foods");
            builder.EntitySet<Order>("Orders");
            builder.EntitySet<OrderDetail>("OrderDetails");
            builder.EntitySet<Payment>("Payments");
            builder.EntitySet<Review>("Reviews");
            builder.EntitySet<User>("Users");
            return builder.GetEdmModel();
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpClient("WEB_API", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7224/api/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = new System.Net.CookieContainer()
                };
            });

            // Add services to the container
            builder.Services.AddDbContext<FoodDeliverContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));

            builder.Services.AddSignalR();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IMessageService, MessageService>();

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

            builder.Services.AddScoped<IPaymentRepositories, PaymentRepositories>();
            builder.Services.AddScoped<IPaymentServices, PaymentServices>();
            builder.Services.AddScoped<VnPayService>();

            builder.Services.AddControllers()
                .AddOData(option => option.Select().Filter()
                    .Count().OrderBy().Expand()
                    .SetMaxTop(100)
                    .AddRouteComponents("odata", GetEdmModel()));

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Cấu hình CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("https://localhost:7266") 
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); 
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowSpecificOrigin"); 
            app.UseSession();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub.ChatHubb>("/chatHub"); 
                endpoints.MapControllers();
            });

            app.Run();
        }
    }
}