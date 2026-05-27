using Microsoft.EntityFrameworkCore;
using PRN232.Lab1.Repositories.Entities;
using PRN232.Lab1.Repositories.Repositories;
using PRN232.Lab1.Services.Services;
using System.IO;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký Lab1DbContext sử dụng SQL Server Connection String
builder.Services.AddDbContext<Lab1DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký Dependency Injection cho Repositories và Services
builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<StudentService>();

builder.Services.AddScoped<SemesterRepository>();
builder.Services.AddScoped<SemesterService>();

builder.Services.AddScoped<CourseRepository>();
builder.Services.AddScoped<CourseService>();

builder.Services.AddScoped<SubjectRepository>();
builder.Services.AddScoped<SubjectService>();

builder.Services.AddScoped<EnrollmentRepository>();
builder.Services.AddScoped<EnrollmentService>();

// Add services to the container.
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<PRN232.Lab1.Services.Mappings.MappingProfile>();
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Enable descriptions for endpoints/parameters/models via XML comments.
    var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);
    if (File.Exists(apiXmlPath))
    {
        options.IncludeXmlComments(apiXmlPath);
    }

    // DTOs live in PRN232.Lab1.Services
    var servicesXmlPath = Path.Combine(AppContext.BaseDirectory, "PRN232.Lab1.Services.xml");
    if (File.Exists(servicesXmlPath))
    {
        options.IncludeXmlComments(servicesXmlPath);
    }
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// --- LOGIC TỰ ĐỘNG KHỞI TẠO VÀ SEED DATA KHI RUN DOCKER ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<Lab1DbContext>();
        // Hàm EnsureCreated() sẽ tự tạo DB dựa trên Fluent API & Seed luôn data từ Bogus nếu DB chưa tồn tại
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Một lỗi đã xảy ra khi tự động tạo dữ liệu mẫu.");
    }
}

// Configure the HTTP request pipeline.
app.UseMiddleware<PRN232.Lab1.API.Middlewares.GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
