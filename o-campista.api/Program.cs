using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.business.imp.Services;
using o_campista.business.IServices;
using o_campista.repository.imp.Repositories;
using o_campista.repository.IRepositories;
using System.Globalization;

var culture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

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
builder.Services.AddHttpClient<StorageService>();
builder.Services.AddScoped<IMapaService, MapaService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IPresenteService, PresenteService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<ICheckinService,CheckinService>();
builder.Services.AddScoped<IConquistaService,ConquistaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ICampingAvaliacaoService, CampingAvaliacaoService>();

//repositories
builder.Services.AddScoped<IUsuarioConquistaRepository,UsuarioConquistaRepository>();
builder.Services.AddScoped<ITrilhaRepository,TrilhaRepository>();
builder.Services.AddScoped<IUsuarioPresenteRepository,UsuarioPresenteRepository>();
builder.Services.AddScoped<ICampingRepository, CampingRepository>();
builder.Services.AddScoped<IUsuarioRepository,UsuarioRepository>();
builder.Services.AddScoped<IPresenteRepository, PresenteRepository>();
builder.Services.AddScoped<ICheckinRepository,CheckinRepository>();
builder.Services.AddScoped<ICampingAvaliacaoRepository, CampingAvaliacaoRepository>();

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
builder.Services.AddDbContext<CampistaDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.UseNetTopologySuite()
    );
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = new[] { culture },
    SupportedUICultures = new[] { culture }
});

app.UseHttpsRedirection();
app.UseCors("AngularPolicy");
app.UseAuthorization();
app.MapControllers();
app.Run();