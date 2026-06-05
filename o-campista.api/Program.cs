using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.business.imp.Services;
using o_campista.business.IServices;
using o_campista.repository.imp.Repositories;
using o_campista.repository.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CampistaDbContext>(
    options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString(
                "DefaultConnection"
            )
        )
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//services
builder.Services.AddScoped<IMapaService, MapaService>();
builder.Services.AddScoped<IAuthService,AuthService>();

//repositories
builder.Services.AddScoped<ICampingRepository, CampingRepository>();
builder.Services.AddScoped<IUsuarioRepository,UsuarioRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AngularPolicy",
        policy =>
        {
            policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
        });
});
builder.Services.AddDbContext<CampistaDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"))
    .EnableSensitiveDataLogging()
    .LogTo(Console.WriteLine);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AngularPolicy");
app.UseAuthorization();
app.MapControllers();
app.Run();