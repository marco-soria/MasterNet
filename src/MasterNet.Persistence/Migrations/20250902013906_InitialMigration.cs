using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MasterNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "instructors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Degree = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_instructors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "prices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "VARCHAR", maxLength: 250, nullable: false),
                    CurrentPrice = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    PromotionalPrice = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "photos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CourseId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_photos_courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ratings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Student = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: false, comment: "Score debe estar entre 1 y 5"),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    CourseId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ratings_courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_instructors",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InstructorId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_instructors", x => new { x.InstructorId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_course_instructors_courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_course_instructors_instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "instructors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "course_prices",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PriceId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_prices", x => new { x.PriceId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_course_prices_courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_course_prices_prices_PriceId",
                        column: x => x.PriceId,
                        principalTable: "prices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "courses",
                columns: new[] { "Id", "Description", "PublicationDate", "Title" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Master modern web development with ASP.NET Core, from basics to advanced concepts including MVC, Web API, and Entity Framework Core.", new DateTime(2024, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Complete ASP.NET Core Web Development" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Deep dive into Entity Framework Core with advanced querying, performance optimization, and database design patterns.", new DateTime(2024, 2, 20, 14, 30, 0, 0, DateTimeKind.Utc), "Advanced Entity Framework Core" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Learn to implement Clean Architecture principles in .NET applications for maintainable and scalable software.", new DateTime(2024, 3, 10, 9, 15, 0, 0, DateTimeKind.Utc), "Clean Architecture with .NET" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Build scalable microservices using .NET, Docker, and modern cloud technologies with hands-on projects.", new DateTime(2024, 4, 5, 16, 45, 0, 0, DateTimeKind.Utc), "Microservices Architecture with .NET" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Master comprehensive testing strategies including unit testing, integration testing, and test-driven development.", new DateTime(2024, 5, 12, 11, 20, 0, 0, DateTimeKind.Utc), "Unit Testing and TDD in .NET" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "Design and develop professional RESTful APIs with authentication, versioning, and comprehensive documentation.", new DateTime(2024, 6, 18, 13, 30, 0, 0, DateTimeKind.Utc), "RESTful Web APIs with ASP.NET Core" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "Create interactive web applications using Blazor Server and WebAssembly with C# instead of JavaScript.", new DateTime(2024, 7, 25, 15, 45, 0, 0, DateTimeKind.Utc), "Blazor: Full-Stack Web Development" },
                    { new Guid("88888888-8888-8888-8888-888888888888"), "Deploy and scale .NET applications in Microsoft Azure with cloud-native development practices.", new DateTime(2024, 8, 8, 12, 15, 0, 0, DateTimeKind.Utc), "Azure Cloud Development with .NET" },
                    { new Guid("99999999-9999-9999-9999-999999999999"), "Learn advanced techniques to optimize .NET application performance, memory management, and scalability.", new DateTime(2024, 9, 14, 10, 30, 0, 0, DateTimeKind.Utc), "Performance Optimization in .NET" }
                });

            migrationBuilder.InsertData(
                table: "instructors",
                columns: new[] { "Id", "Degree", "FirstName", "LastName" },
                values: new object[,]
                {
                    { new Guid("12345678-1234-1234-1234-123456789012"), "PhD in Software Engineering", "Sarah", "Williams" },
                    { new Guid("23456789-2345-2345-2345-234567890123"), "Bachelor of Computer Engineering", "Michael", "Johnson" },
                    { new Guid("34567890-3456-3456-3456-345678901234"), "Master of Information Technology", "Emily", "Davis" },
                    { new Guid("45678901-4567-4567-4567-456789012345"), "Certified Solutions Architect", "Robert", "Brown" },
                    { new Guid("56789012-5678-5678-5678-567890123456"), "Master of Business Administration", "Jennifer", "Miller" },
                    { new Guid("67890123-6789-6789-6789-678901234567"), "PhD in Computer Science", "David", "Wilson" },
                    { new Guid("78901234-7890-7890-7890-789012345678"), "Senior Software Developer", "Lisa", "Garcia" },
                    { new Guid("89012345-8901-8901-8901-890123456789"), "Cloud Solutions Expert", "Christopher", "Martinez" },
                    { new Guid("90123456-9012-9012-9012-901234567890"), "DevOps Engineering Specialist", "Amanda", "Taylor" },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "Master of Computer Science", "John", "Anderson" }
                });

            migrationBuilder.InsertData(
                table: "prices",
                columns: new[] { "Id", "CurrentPrice", "Name", "PromotionalPrice" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 49.99m, "Basic Tier", 39.99m },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 99.99m, "Standard Tier", 79.99m },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), 149.99m, "Premium Tier", 119.99m },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), 29.99m, "Student Discount", 19.99m },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), 299.99m, "Enterprise License", 249.99m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseInstructor_CourseId",
                table: "course_instructors",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseInstructor_InstructorId",
                table: "course_instructors",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrice_CourseId",
                table: "course_prices",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrice_PriceId",
                table: "course_prices",
                column: "PriceId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_CourseId",
                table: "photos",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_CourseId",
                table: "ratings",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_instructors");

            migrationBuilder.DropTable(
                name: "course_prices");

            migrationBuilder.DropTable(
                name: "photos");

            migrationBuilder.DropTable(
                name: "ratings");

            migrationBuilder.DropTable(
                name: "instructors");

            migrationBuilder.DropTable(
                name: "prices");

            migrationBuilder.DropTable(
                name: "courses");
        }
    }
}
