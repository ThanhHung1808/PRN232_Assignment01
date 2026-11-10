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
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.SwaggerGen;

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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "FUNewsManagementAPI",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "FUNewsManagementClient",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
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
        Title = "FUNewsManagement API - Assignment 01", 
        Version = "v1",
        Description = "ASP.NET Core Web API with OData for FUNewsManagement System (PRN232 Assignment 01)"
    });

    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.OperationFilter<AuthorizeRoleOperationFilter>();

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

public class AuthorizeRoleOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        var authAttributes = actionMetadata.OfType<AuthorizeAttribute>().ToList();
        var allowAnonymous = actionMetadata.OfType<AllowAnonymousAttribute>().Any();

        string roleTag;
        if (allowAnonymous || !authAttributes.Any())
        {
            roleTag = "(Public)";
        }
        else
        {
            var roles = authAttributes
                .Where(a => !string.IsNullOrEmpty(a.Roles))
                .SelectMany(a => a.Roles!.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .ToList();

            if (roles.Any())
            {
                roleTag = $"({string.Join(", ", roles)})";
            }
            else
            {
                roleTag = "(Authenticated)";
            }
        }

        var methodName = context.MethodInfo.Name;
        var friendlyName = methodName switch
        {
            "GetAccounts" => "Lấy danh sách tài khoản",
            "GetAccount" => "Lấy thông tin tài khoản",
            "CreateAccount" => "Tạo mới tài khoản",
            "UpdateAccount" => "Cập nhật tài khoản",
            "DeleteAccount" => "Xóa tài khoản",
            "UpdateProfile" => "Cập nhật hồ sơ cá nhân",
            "GetArticles" => "Lấy danh sách bài báo",
            "GetArticle" => "Lấy chi tiết bài báo",
            "GetArticlesByAuthor" => "Lấy bài báo theo tác giả",
            "CreateArticle" => "Tạo mới bài báo",
            "UpdateArticle" => "Cập nhật bài báo",
            "DeleteArticle" => "Xóa bài báo",
            "GetCategories" => "Lấy danh sách danh mục",
            "GetCategory" => "Lấy chi tiết danh mục",
            "CreateCategory" => "Tạo mới danh mục",
            "UpdateCategory" => "Cập nhật danh mục",
            "DeleteCategory" => "Xóa danh mục",
            "GetTags" => "Lấy danh sách thẻ Tag",
            "GetTag" => "Lấy chi tiết thẻ Tag",
            "CreateTag" => "Tạo mới thẻ Tag",
            "UpdateTag" => "Cập nhật thẻ Tag",
            "DeleteTag" => "Xóa thẻ Tag",
            "Login" => "Đăng nhập hệ thống",
            "CheckAuthStatus" => "Kiểm tra trạng thái xác thực",
            "GetStatistics" => "Báo cáo thống kê tin tức",
            _ => methodName
        };

        operation.Summary = $"{friendlyName} {roleTag}";
        operation.Description = $"Quyền thực thi: {roleTag.Replace("(", "").Replace(")", "")}. " + (operation.Description ?? "");
    }
}
