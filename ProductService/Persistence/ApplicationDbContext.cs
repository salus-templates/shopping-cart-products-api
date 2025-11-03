using Microsoft.EntityFrameworkCore;

namespace ProductService.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; } = null!;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(builder);
        builder.Entity<Product>().HasData(
            new Product
            {
                Id = Guid.Parse("d7b8b57c-5d7b-4e8a-9a9c-b2b7d2b21d9f"),
                Name = "Wireless Headphones",
                Price = 99.99m,
                ImageUrl = "https://placehold.co/300x200/FFD700/000000?text=Headphones",
                Description = "High-fidelity sound with noise cancellation.",
                Stock = 10
            },
            new Product
            {
                Id = Guid.Parse("c2a9133d-3e52-4c90-bb9d-bbc7b5284b44"),
                Name = "Smartwatch",
                Price = 199.99m,
                ImageUrl = "https://placehold.co/300x200/87CEEB/000000?text=Smartwatch",
                Description = "Track your fitness and receive notifications.",
                Stock = 5
            },
            new Product
            {
                Id = Guid.Parse("0e3e8e64-df62-4b2e-b34a-7f60b54b07c3"),
                Name = "Portable Bluetooth Speaker",
                Price = 49.99m,
                ImageUrl = "https://placehold.co/300x200/98FB98/000000?text=Speaker",
                Description = "Compact and powerful sound on the go.",
                Stock = 20
            },
            new Product
            {
                Id = Guid.Parse("e8a7b9c2-84b1-4a12-9350-fb2caa1b1cdd"),
                Name = "Ergonomic Office Chair",
                Price = 249.99m,
                ImageUrl = "https://placehold.co/300x200/DDA0DD/000000?text=Chair",
                Description = "Comfortable and supportive for long working hours.",
                Stock = 3
            },
            new Product
            {
                Id = Guid.Parse("f43a1a52-6b42-4dc3-9c2b-2d238fb707f5"),
                Name = "4K UHD Monitor",
                Price = 399.99m,
                ImageUrl = "https://placehold.co/300x200/F08080/000000?text=Monitor",
                Description = "Stunning visuals for work and entertainment.",
                Stock = 8
            },
            new Product
            {
                Id = Guid.Parse("b58d67f1-d95f-469d-9b18-bac2b8b6d8f2"),
                Name = "Gaming Keyboard",
                Price = 79.99m,
                ImageUrl = "https://placehold.co/300x200/ADD8E6/000000?text=Keyboard",
                Description = "Mechanical keyboard with RGB lighting.",
                Stock = 15
            },
            new Product
            {
                Id = Guid.Parse("b8f21e22-2e39-4bb0-9a49-bb1a99303b9f"),
                Name = "Gaming Mouse",
                Price = 39.99m,
                ImageUrl = "https://placehold.co/300x200/FFB6C1/000000?text=Mouse",
                Description = "High-precision sensor for competitive gaming.",
                Stock = 12
            },
            new Product
            {
                Id = Guid.Parse("1a4d8b0a-2b7c-44a3-96b4-c11f0a1d7a77"),
                Name = "Webcam 1080p",
                Price = 59.99m,
                ImageUrl = "https://placehold.co/300x200/DAA520/000000?text=Webcam",
                Description = "Full HD video calls and streaming.",
                Stock = 7
            },
            new Product
            {
                Id = Guid.Parse("3c51a78b-7f42-4e2c-9f1a-bd7b56e79f51"),
                Name = "External SSD 1TB",
                Price = 129.99m,
                ImageUrl = "https://placehold.co/300x200/B0C4DE/000000?text=SSD",
                Description = "Fast and portable storage solution.",
                Stock = 4
            },
            new Product
            {
                Id = Guid.Parse("a60a9a16-5f74-4d92-8c2d-7f6d44ef9b60"),
                Name = "USB-C Hub",
                Price = 29.99m,
                ImageUrl = "https://placehold.co/300x200/F4A460/000000?text=USB-C+Hub",
                Description = "Expand your laptop's connectivity with multiple ports.",
                Stock = 25
            },
            new Product
            {
                Id = Guid.Parse("cf86c4f3-96f3-4f1e-982a-47b3a5f71f5b"),
                Name = "Noise-Cancelling Earbuds",
                Price = 129.99m,
                ImageUrl = "https://placehold.co/300x200/C0C0C0/000000?text=Earbuds",
                Description = "Compact earbuds with active noise cancellation.",
                Stock = 9
            },
            new Product
            {
                Id = Guid.Parse("0c3f4d82-13b7-4e49-a45b-25db07a3c631"),
                Name = "Smart Home Hub",
                Price = 89.99m,
                ImageUrl = "https://placehold.co/300x200/D8BFD8/000000?text=Smart+Hub",
                Description = "Control all your smart devices from one place.",
                Stock = 6
            },
            new Product
            {
                Id = Guid.Parse("b0a4a3cc-4c0a-4b73-97cb-cb9a344a1d10"),
                Name = "Robot Vacuum Cleaner",
                Price = 299.99m,
                ImageUrl = "https://placehold.co/300x200/AFEEEE/000000?text=Vacuum",
                Description = "Automated cleaning for a spotless home.",
                Stock = 2
            },
            new Product
            {
                Id = Guid.Parse("ee14b35d-2a3a-46e3-a47b-f3a1d35117a9"),
                Name = "Digital Camera",
                Price = 499.99m,
                ImageUrl = "https://placehold.co/300x200/F5DEB3/000000?text=Camera",
                Description = "Capture stunning photos and videos.",
                Stock = 3
            },
            new Product
            {
                Id = Guid.Parse("ac71c501-b889-4742-b41e-61cfc9c3b91a"),
                Name = "Portable Projector",
                Price = 179.99m,
                ImageUrl = "https://placehold.co/300x200/9ACD32/000000?text=Projector",
                Description = "Enjoy movies anywhere with a compact projector.",
                Stock = 5
            },
            new Product
            {
                Id = Guid.Parse("a3bc612e-1ebd-4d12-95db-748147a4b6e4"),
                Name = "Fitness Tracker",
                Price = 69.99m,
                ImageUrl = "https://placehold.co/300x200/FFA07A/000000?text=Fitness+Tracker",
                Description = "Monitor your activity, heart rate, and sleep.",
                Stock = 18
            },
            new Product
            {
                Id = Guid.Parse("47a13d15-9e37-4e3d-8c59-48289ac62d6c"),
                Name = "Electric Toothbrush",
                Price = 45.99m,
                ImageUrl = "https://placehold.co/300x200/BDB76B/000000?text=Toothbrush",
                Description = "Advanced cleaning for healthier gums.",
                Stock = 14
            },
            new Product
            {
                Id = Guid.Parse("8e5130b7-0932-4fa4-97b9-4b68d7c5b07f"),
                Name = "Air Fryer",
                Price = 89.99m,
                ImageUrl = "https://placehold.co/300x200/E0FFFF/000000?text=Air+Fryer",
                Description = "Cook healthier meals with less oil.",
                Stock = 10
            },
            new Product
            {
                Id = Guid.Parse("22e7b923-ff07-4c92-bc85-8cc3b4ad72b1"),
                Name = "Coffee Maker",
                Price = 75.99m,
                ImageUrl = "https://placehold.co/300x200/D2B48C/000000?text=Coffee+Maker",
                Description = "Brew your perfect cup of coffee every morning.",
                Stock = 8
            },
            new Product
            {
                Id = Guid.Parse("0b73a09f-b47f-4ac0-80a9-d91ed9f24c85"),
                Name = "Smart Light Bulbs (2-pack)",
                Price = 25.99m,
                ImageUrl = "https://placehold.co/300x200/F0F8FF/000000?text=Smart+Bulbs",
                Description = "Control your lighting with your voice or app.",
                Stock = 30
            }
        );
    }
}