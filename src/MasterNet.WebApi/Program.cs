using MasterNet.Application;
using MasterNet.Application.Interfaces;
using MasterNet.Infrastructure.Photos;
using MasterNet.Infrastructure.Reports;
using MasterNet.Persistence;
using MasterNet.Persistence.Extensions;
using MasterNet.Persistence.Models;
using MasterNet.WebApi.Extensions;
using MasterNet.WebApi.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddPoliciesServices();

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection(nameof(CloudinarySettings)));

builder.Services.AddScoped<IPhotoService, PhotoService>();

//builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped(typeof(IReportService<>), typeof(ReportService<>));

builder.Services.AddHttpContextAccessor();


builder.Services.AddControllers();

builder.Services.AddSwaggerDocumentation();

builder.Services.AddCors(o => o.AddPolicy("cors-app", builder => {
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));



var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.useSwaggerDocumentation();

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseCors("cors-app");

// ✅ Database initialization usando método de extensión
await app.ApplyDatabaseInitializationAsync();

app.MapControllers();

app.Run();
