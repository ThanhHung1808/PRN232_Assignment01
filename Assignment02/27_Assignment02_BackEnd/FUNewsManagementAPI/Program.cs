using System.Text;
using FUNewsManagementAPI.DAO;
using FUNewsManagementAPI.DTOs;
using FUNewsManagementAPI.Models;
using FUNewsManagementAPI.Repositories.Implementations;
using FUNewsManagementAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<AdminAccountConfig>(builder.Configuration.GetSection("AdminAccount"));

// Register DbContext with ConnectionString "MyCnn"
builder.Services.AddDbContext<FUNewsManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

// Repositories DI
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// OData configuration
static IEdmModel GetEdmModel()
{
    var modelBuilder = new ODataConventionModelBuilder();
    var categories = modelBuilder.EntitySet<CategoryResponseDto>("Categories");
    categories.EntityType.HasKey(c => c.CategoryId);

    var articles = modelBuilder.EntitySet<NewsArticleResponseDto>("NewsArticles");
    articles.EntityType.HasKey(n => n.NewsArticleId);

    var accounts = modelBuilder.EntitySet<AccountResponseDto>("Accounts");
    accounts.EntityType.HasKey(a => a.AccountId);

    modelBuilder.EntitySet<Tag>("Tags");
    return modelBuilder.GetEdmModel();
}

builder.Services.AddControllers()
    .AddOData(options =>
    {
        options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100);
        options.AddRouteComponents("odata", GetEdmModel());
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "PRN232_FUNewsManagement_SecretKey_For_Assignment01_2026_FPT!";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "FUNewsManagementAPI",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "FUNewsManagementClient",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        NameClaimType = System.Security.Claims.ClaimTypes.Name
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var result = new
            {
                statusCode = 401,
                status = "CHƯA XÁC THỰC (UNAUTHENTICATED)",
                message = "Tài khoản chưa được xác thực hoặc Token không hợp lệ! Vui lòng đăng nhập tại POST /api/Auth/login và dán mã Token vào nút Authorize trên Swagger (lưu ý: chỉ dán chuỗi token, không kèm dấu ngoặc nhọn {} hoặc từ khóa Bearer)."
            };
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(result));
        },
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var result = new
            {
                statusCode = 403,
                status = "KHÔNG CÓ QUYỀN (FORBIDDEN)",
                message = "Tài khoản đã xác thực nhưng không có quyền (Role) phù hợp để truy cập chức năng này."
            };
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(result));
        }
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "FUNewsManagement API - Assignment 02 (Security Web API)", 
        Version = "v1",
        Description = "ASP.NET Core Web API with OData & JWT Authentication (PRN232 Assignment 02)"
    });

    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Paste your JWT token here (without 'Bearer ' prefix). Example: eyJhbGciOi...",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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

app.UseDeveloperExceptionPage();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FUNewsManagement API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
