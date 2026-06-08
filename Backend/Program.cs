using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PaymentService.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Port Configuration (Running on 5000 and 5003 if not hosted in IIS)
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_PORT")))
{
    builder.WebHost.UseUrls("http://*:5000;http://*:5003");
}

// Add Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=PaymentDB;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Key"] ?? "DNU_Finance_Super_Secret_JWT_Key_2026_FIT_DNU";
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "DNUFinance",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "DNUAudience",
        ValidateLifetime = true,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        NameClaimType = System.Security.Claims.ClaimTypes.Name
    };
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure HttpClients for integration placeholder services
builder.Services.AddHttpClient<PaymentService.Services.ICourseServiceClient, PaymentService.Services.CourseServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Integration:CourseServiceUrl"] ?? "http://localhost:5001/api/");
});

builder.Services.AddHttpClient<PaymentService.Services.IClassServiceClient, PaymentService.Services.ClassServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Integration:CourseServiceUrl"] ?? "http://localhost:5001/api/");
});

builder.Services.AddHttpClient<PaymentService.Services.IStudentServiceClient, PaymentService.Services.StudentServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Integration:StudentServiceUrl"] ?? "http://localhost:5002/api/");
});

builder.Services.AddHttpClient<PaymentService.Services.IRegistrationServiceClient, PaymentService.Services.RegistrationServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Integration:StudentServiceUrl"] ?? "http://localhost:5002/api/");
});

builder.Services.AddControllers();

// Add Swagger Support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DNU Payment & Report Service API", Version = "v1" });
    
    // Add JWT Token support in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập JWT Bearer Token theo cú pháp: 'Bearer [token]'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Auto-create database and seed data on startup (Zero-Configuration setup!)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PaymentDbContext>();
        context.Database.EnsureCreated(); // Creates DB and seeds if not exists

        // Ensure maitt's role is updated to Staff if it was seeded as Admin previously
        var maitt = context.Users.FirstOrDefault(u => u.Username == "maitt");
        if (maitt != null && maitt.Role == "Admin")
        {
            maitt.Role = "Staff";
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khi tự khởi tạo cơ sở dữ liệu và đổ seed data.");
    }
}

// Enable Swagger UI always to make testing super friendly
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment API v1");
    c.RoutePrefix = "swagger"; // Opens swagger at http://localhost:5003/swagger
});

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();