using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MasterNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentitySystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "app_roles",
                keyColumn: "Id",
                keyValue: "0bc6d645-46c9-4318-a4fa-175553e302ea");

            migrationBuilder.DeleteData(
                table: "app_roles",
                keyColumn: "Id",
                keyValue: "d342281e-3be7-4bc1-ac77-91190d6e1a47");

            migrationBuilder.DeleteData(
                table: "courses",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "courses",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "courses",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "courses",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "courses",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"));

            migrationBuilder.DeleteData(
                table: "courses",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));

            migrationBuilder.DeleteData(
                table: "instructors",
                keyColumn: "Id",
                keyValue: new Guid("23456789-2345-2345-2345-234567890123"));

            migrationBuilder.DeleteData(
                table: "instructors",
                keyColumn: "Id",
                keyValue: new Guid("34567890-3456-3456-3456-345678901234"));

            migrationBuilder.DeleteData(
                table: "instructors",
                keyColumn: "Id",
                keyValue: new Guid("45678901-4567-4567-4567-456789012345"));

            migrationBuilder.DeleteData(
                table: "instructors",
                keyColumn: "Id",
                keyValue: new Guid("56789012-5678-5678-5678-567890123456"));

            migrationBuilder.DeleteData(
                table: "instructors",
                keyColumn: "Id",
                keyValue: new Guid("67890123-6789-6789-6789-678901234567"));

            migrationBuilder.DeleteData(
                table: "instructors",
                keyColumn: "Id",
                keyValue: new Guid("78901234-7890-7890-7890-789012345678"));

            migrationBuilder.DeleteData(
                table: "instructors",
                keyColumn: "Id",
                keyValue: new Guid("89012345-8901-8901-8901-890123456789"));

            migrationBuilder.DeleteData(
                table: "instructors",
                keyColumn: "Id",
                keyValue: new Guid("90123456-9012-9012-9012-901234567890"));

            migrationBuilder.DeleteData(
                table: "prices",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "prices",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));

            migrationBuilder.DeleteData(
                table: "prices",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"));

            migrationBuilder.AlterColumn<int>(
                name: "Score",
                table: "ratings",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldComment: "Score debe estar entre 1 y 5");

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 1,
                column: "RoleId",
                value: "ADMIN-ROLE-ID-12345678-1234-1234-1234-123456789012");

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "COURSE_WRITE", "ADMIN-ROLE-ID-12345678-1234-1234-1234-123456789012" });

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "COURSE_UPDATE", "ADMIN-ROLE-ID-12345678-1234-1234-1234-123456789012" });

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 4,
                column: "RoleId",
                value: "ADMIN-ROLE-ID-12345678-1234-1234-1234-123456789012");

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "INSTRUCTOR_READ", "ADMIN-ROLE-ID-12345678-1234-1234-1234-123456789012" });

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "INSTRUCTOR_CREATE", "ADMIN-ROLE-ID-12345678-1234-1234-1234-123456789012" });

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 7,
                column: "RoleId",
                value: "ADMIN-ROLE-ID-12345678-1234-1234-1234-123456789012");

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 8,
                column: "RoleId",
                value: "ADMIN-ROLE-ID-12345678-1234-1234-1234-123456789012");

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "COMMENT_CREATE", "ADMIN-ROLE-ID-12345678-1234-1234-1234-123456789012" });

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "COMMENT_DELETE", "ADMIN-ROLE-ID-12345678-1234-1234-1234-123456789012" });

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 11,
                column: "RoleId",
                value: "CLIENT-ROLE-ID-87654321-4321-4321-4321-210987654321");

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 12,
                column: "RoleId",
                value: "CLIENT-ROLE-ID-87654321-4321-4321-4321-210987654321");

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 13,
                column: "RoleId",
                value: "CLIENT-ROLE-ID-87654321-4321-4321-4321-210987654321");

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 14,
                column: "RoleId",
                value: "CLIENT-ROLE-ID-87654321-4321-4321-4321-210987654321");

            migrationBuilder.InsertData(
                table: "app_roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "ADMIN-ROLE-ID-12345678-1234-1234-1234-123456789012", "ADMIN-STAMP-12345", "ADMIN", "ADMIN" },
                    { "CLIENT-ROLE-ID-87654321-4321-4321-4321-210987654321", "CLIENT-STAMP-54321", "CLIENT", "CLIENT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "app_roles",
                keyColumn: "Id",
                keyValue: "ADMIN-ROLE-ID-12345678-1234-1234-1234-123456789012");

            migrationBuilder.DeleteData(
                table: "app_roles",
                keyColumn: "Id",
                keyValue: "CLIENT-ROLE-ID-87654321-4321-4321-4321-210987654321");

            migrationBuilder.AlterColumn<int>(
                name: "Score",
                table: "ratings",
                type: "INTEGER",
                nullable: false,
                comment: "Score debe estar entre 1 y 5",
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 1,
                column: "RoleId",
                value: "d342281e-3be7-4bc1-ac77-91190d6e1a47");

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "COURSE_UPDATE", "d342281e-3be7-4bc1-ac77-91190d6e1a47" });

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "COURSE_WRITE", "d342281e-3be7-4bc1-ac77-91190d6e1a47" });

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 4,
                column: "RoleId",
                value: "d342281e-3be7-4bc1-ac77-91190d6e1a47");

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "INSTRUCTOR_CREATE", "d342281e-3be7-4bc1-ac77-91190d6e1a47" });

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "INSTRUCTOR_READ", "d342281e-3be7-4bc1-ac77-91190d6e1a47" });

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 7,
                column: "RoleId",
                value: "d342281e-3be7-4bc1-ac77-91190d6e1a47");

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 8,
                column: "RoleId",
                value: "d342281e-3be7-4bc1-ac77-91190d6e1a47");

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "COMMENT_DELETE", "d342281e-3be7-4bc1-ac77-91190d6e1a47" });

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ClaimValue", "RoleId" },
                values: new object[] { "COMMENT_CREATE", "d342281e-3be7-4bc1-ac77-91190d6e1a47" });

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 11,
                column: "RoleId",
                value: "0bc6d645-46c9-4318-a4fa-175553e302ea");

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 12,
                column: "RoleId",
                value: "0bc6d645-46c9-4318-a4fa-175553e302ea");

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 13,
                column: "RoleId",
                value: "0bc6d645-46c9-4318-a4fa-175553e302ea");

            migrationBuilder.UpdateData(
                table: "app_role_claims",
                keyColumn: "Id",
                keyValue: 14,
                column: "RoleId",
                value: "0bc6d645-46c9-4318-a4fa-175553e302ea");

            migrationBuilder.InsertData(
                table: "app_roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0bc6d645-46c9-4318-a4fa-175553e302ea", null, "CLIENT", "CLIENT" },
                    { "d342281e-3be7-4bc1-ac77-91190d6e1a47", null, "ADMIN", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "courses",
                columns: new[] { "Id", "Description", "PublicationDate", "Title" },
                values: new object[,]
                {
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
                    { new Guid("23456789-2345-2345-2345-234567890123"), "Bachelor of Computer Engineering", "Michael", "Johnson" },
                    { new Guid("34567890-3456-3456-3456-345678901234"), "Master of Information Technology", "Emily", "Davis" },
                    { new Guid("45678901-4567-4567-4567-456789012345"), "Certified Solutions Architect", "Robert", "Brown" },
                    { new Guid("56789012-5678-5678-5678-567890123456"), "Master of Business Administration", "Jennifer", "Miller" },
                    { new Guid("67890123-6789-6789-6789-678901234567"), "PhD in Computer Science", "David", "Wilson" },
                    { new Guid("78901234-7890-7890-7890-789012345678"), "Senior Software Developer", "Lisa", "Garcia" },
                    { new Guid("89012345-8901-8901-8901-890123456789"), "Cloud Solutions Expert", "Christopher", "Martinez" },
                    { new Guid("90123456-9012-9012-9012-901234567890"), "DevOps Engineering Specialist", "Amanda", "Taylor" }
                });

            migrationBuilder.InsertData(
                table: "prices",
                columns: new[] { "Id", "CurrentPrice", "Name", "PromotionalPrice" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), 149.99m, "Premium Tier", 119.99m },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), 29.99m, "Student Discount", 19.99m },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), 299.99m, "Enterprise License", 249.99m }
                });
        }
    }
}
