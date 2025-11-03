using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductService.Migrations
{
    /// <inheritdoc />
    public partial class AddProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "ImageUrl", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { new Guid("0b73a09f-b47f-4ac0-80a9-d91ed9f24c85"), "Control your lighting with your voice or app.", "https://placehold.co/300x200/F0F8FF/000000?text=Smart+Bulbs", "Smart Light Bulbs (2-pack)", 25.99m, 30 },
                    { new Guid("0c3f4d82-13b7-4e49-a45b-25db07a3c631"), "Control all your smart devices from one place.", "https://placehold.co/300x200/D8BFD8/000000?text=Smart+Hub", "Smart Home Hub", 89.99m, 6 },
                    { new Guid("0e3e8e64-df62-4b2e-b34a-7f60b54b07c3"), "Compact and powerful sound on the go.", "https://placehold.co/300x200/98FB98/000000?text=Speaker", "Portable Bluetooth Speaker", 49.99m, 20 },
                    { new Guid("1a4d8b0a-2b7c-44a3-96b4-c11f0a1d7a77"), "Full HD video calls and streaming.", "https://placehold.co/300x200/DAA520/000000?text=Webcam", "Webcam 1080p", 59.99m, 7 },
                    { new Guid("22e7b923-ff07-4c92-bc85-8cc3b4ad72b1"), "Brew your perfect cup of coffee every morning.", "https://placehold.co/300x200/D2B48C/000000?text=Coffee+Maker", "Coffee Maker", 75.99m, 8 },
                    { new Guid("3c51a78b-7f42-4e2c-9f1a-bd7b56e79f51"), "Fast and portable storage solution.", "https://placehold.co/300x200/B0C4DE/000000?text=SSD", "External SSD 1TB", 129.99m, 4 },
                    { new Guid("47a13d15-9e37-4e3d-8c59-48289ac62d6c"), "Advanced cleaning for healthier gums.", "https://placehold.co/300x200/BDB76B/000000?text=Toothbrush", "Electric Toothbrush", 45.99m, 14 },
                    { new Guid("8e5130b7-0932-4fa4-97b9-4b68d7c5b07f"), "Cook healthier meals with less oil.", "https://placehold.co/300x200/E0FFFF/000000?text=Air+Fryer", "Air Fryer", 89.99m, 10 },
                    { new Guid("a3bc612e-1ebd-4d12-95db-748147a4b6e4"), "Monitor your activity, heart rate, and sleep.", "https://placehold.co/300x200/FFA07A/000000?text=Fitness+Tracker", "Fitness Tracker", 69.99m, 18 },
                    { new Guid("a60a9a16-5f74-4d92-8c2d-7f6d44ef9b60"), "Expand your laptop's connectivity with multiple ports.", "https://placehold.co/300x200/F4A460/000000?text=USB-C+Hub", "USB-C Hub", 29.99m, 25 },
                    { new Guid("ac71c501-b889-4742-b41e-61cfc9c3b91a"), "Enjoy movies anywhere with a compact projector.", "https://placehold.co/300x200/9ACD32/000000?text=Projector", "Portable Projector", 179.99m, 5 },
                    { new Guid("b0a4a3cc-4c0a-4b73-97cb-cb9a344a1d10"), "Automated cleaning for a spotless home.", "https://placehold.co/300x200/AFEEEE/000000?text=Vacuum", "Robot Vacuum Cleaner", 299.99m, 2 },
                    { new Guid("b58d67f1-d95f-469d-9b18-bac2b8b6d8f2"), "Mechanical keyboard with RGB lighting.", "https://placehold.co/300x200/ADD8E6/000000?text=Keyboard", "Gaming Keyboard", 79.99m, 15 },
                    { new Guid("b8f21e22-2e39-4bb0-9a49-bb1a99303b9f"), "High-precision sensor for competitive gaming.", "https://placehold.co/300x200/FFB6C1/000000?text=Mouse", "Gaming Mouse", 39.99m, 12 },
                    { new Guid("c2a9133d-3e52-4c90-bb9d-bbc7b5284b44"), "Track your fitness and receive notifications.", "https://placehold.co/300x200/87CEEB/000000?text=Smartwatch", "Smartwatch", 199.99m, 5 },
                    { new Guid("cf86c4f3-96f3-4f1e-982a-47b3a5f71f5b"), "Compact earbuds with active noise cancellation.", "https://placehold.co/300x200/C0C0C0/000000?text=Earbuds", "Noise-Cancelling Earbuds", 129.99m, 9 },
                    { new Guid("d7b8b57c-5d7b-4e8a-9a9c-b2b7d2b21d9f"), "High-fidelity sound with noise cancellation.", "https://placehold.co/300x200/FFD700/000000?text=Headphones", "Wireless Headphones", 99.99m, 10 },
                    { new Guid("e8a7b9c2-84b1-4a12-9350-fb2caa1b1cdd"), "Comfortable and supportive for long working hours.", "https://placehold.co/300x200/DDA0DD/000000?text=Chair", "Ergonomic Office Chair", 249.99m, 3 },
                    { new Guid("ee14b35d-2a3a-46e3-a47b-f3a1d35117a9"), "Capture stunning photos and videos.", "https://placehold.co/300x200/F5DEB3/000000?text=Camera", "Digital Camera", 499.99m, 3 },
                    { new Guid("f43a1a52-6b42-4dc3-9c2b-2d238fb707f5"), "Stunning visuals for work and entertainment.", "https://placehold.co/300x200/F08080/000000?text=Monitor", "4K UHD Monitor", 399.99m, 8 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
