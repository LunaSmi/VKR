using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks.Dataflow;
using VKR.API;
using VKR.API.Configs;
using VKR.API.Services;
using Microsoft.OpenApi.Models;
using VKR.API.Extensions;
using VKR.API.Mapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var authSection = builder.Configuration.GetSection(AuthConfig.Position);
var authConfig =authSection.Get<AuthConfig>();
builder.Services.Configure<AuthConfig>(authSection);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "??????? ????? ????????????",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,

    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme,

                        },
                        Scheme = "oauth2",
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });

    c.SwaggerDoc("Auth", new OpenApiInfo { Title = "Auth" });
    c.SwaggerDoc("API", new OpenApiInfo { Title = "API" });
});

builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<PostsService>();
builder.Services.AddScoped<AttachService>();
builder.Services.AddScoped<LinkGeneratorService>();

builder.Services.AddDbContext<VKR.DAL.DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"), sql => { });
});

builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

})
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer=true,
            ValidIssuer=authConfig.Issuer,
            ValidateAudience=true,
            ValidAudience=authConfig.Audience,
            ValidateLifetime=true,
            ValidateIssuerSigningKey=true,
            IssuerSigningKey = authConfig.SimmetricSecurityKey(),
            ClockSkew = TimeSpan.Zero
        };
    }) ;

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("ValidAccessToken", p =>
    {
        p.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        p.RequireAuthenticatedUser();
    });
});

var app = builder.Build();



using(var serviceScope = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
{
    if(serviceScope != null)
    {
        var context = serviceScope.ServiceProvider.GetRequiredService<VKR.DAL.DataContext>();
        context.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("API/swagger.json", "API");
        c.SwaggerEndpoint("Auth/swagger.json", "Auth");
    });
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseTokenValidation();
app.UseExceptionMiddleware();
app.MapControllers();

app.Run();
