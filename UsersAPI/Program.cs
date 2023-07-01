using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.CodeAnalysis.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using UsersAPI.Configurations;
//using Microsoft.OpenApi.Filters;
using UsersAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddSwaggerGen(options =>
//{
//    options.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//    {
//        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
//        Name = "Authorization",
//        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
//    });
//    options.OperationFilter<SecurityRequirementsOperationFilter>();
//});
//builder.Services.AddAuthentication().AddJwtBearer();

// Added code
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value!);

    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // for dev
        ValidateAudience = false, // for dev
        RequireExpirationTime = false, // no refresh tokens
        ValidateLifetime = true,
    };
});

builder.Services.AddResponseCompression(options => options.EnableForHttps = true);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<CrmContext>();

var MyAllowSpecificOrigins = "MyPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:7056").AllowAnyHeader().AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// Added
app.UseAuthentication();

app.UseResponseCompression();
app.UseCors(MyAllowSpecificOrigins);

app.Run();
