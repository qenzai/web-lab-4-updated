using Microsoft.EntityFrameworkCore;
using MyWebsiteBackend.Models;

namespace MyWebsiteBackend.Data
{
    public static class DbSeeder
    {
        public const string ImagesPath = "wwwroot/images/";

        public static void Seed(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Database.Migrate();

            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Відеокарти" },
                    new Category { Name = "Процесори" },
                    new Category { Name = "Оперативна пам’ять" },
                    new Category { Name = "Накопичувачі" }
                };

                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            if (!context.Products.Any())
            {
                var videoCards = context.Categories.First(c => c.Name == "Відеокарти");
                var cpus = context.Categories.First(c => c.Name == "Процесори");
                var ram = context.Categories.First(c => c.Name == "Оперативна пам’ять");
                var drives = context.Categories.First(c => c.Name == "Накопичувачі");

                var products = new List<Product>
                // dotnet ef database update ( обновляє картинки )
                {
                    new Product { Name = "NVIDIA RTX 3080", Price = 1200, Desc = "Потужна відеокарта для геймінгу", Extra = "Флагманська модель від NVIDIA", Img = "/public/images/1.jpg", Category = videoCards },
                    new Product { Name = "AMD RX 6800 XT", Price = 1000, Desc = "Високопродуктивна відеокарта", Extra = "Оптимальний вибір для геймерів", Img = "/public/images/2.jpg", Category = videoCards },

                    new Product { Name = "Intel i9-12900K", Price = 680, Desc = "Швидкий багатоядерний процесор", Extra = "Флагманський процесор Intel", Img = "/public/images/3.jpg", Category = cpus },
                    new Product { Name = "AMD Ryzen 9 5900X", Price = 700, Desc = "Відмінний вибір для роботи і ігор", Extra = "Топовий процесор AMD", Img = "/public/images/4.jpg", Category = cpus },

                    new Product { Name = "Corsair 32GB DDR5", Price = 400, Desc = "Високошвидкісна оперативна пам’ять", Extra = "Новітня технологія DDR5", Img = "/public/images/5.jpg", Category = ram },
                    new Product { Name = "G.Skill 16GB DDR4", Price = 340, Desc = "Гарний вибір для будь-яких задач", Extra = "Оптимальна швидкість і надійність", Img = "/public/images/6.jpg", Category = ram },

                    new Product { Name = "Samsung 980 PRO 1TB", Price = 800, Desc = "Надшвидкий SSD диск", Extra = "Найкращий вибір для ентузіастів", Img = "/public/images/7.jpg", Category = drives },
                    new Product { Name = "WD Black SN850 2TB", Price = 550, Desc = "Висока швидкість і надійність", Extra = "Максимальна продуктивність для ігор", Img = "/public/images/8.jpg", Category = drives },
        };

                

                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }
    }
}
