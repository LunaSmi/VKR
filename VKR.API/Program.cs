using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks.Dataflow;
using VKR.API;
using VKR.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);

builder.Services.AddScoped<UsersService>();

builder.Services.AddDbContext<VKR.DAL.DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"), sql => { });
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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
