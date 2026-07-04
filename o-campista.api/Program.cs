using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using o_campista.api.Context;
using o_campista.api.Hubs;
using o_campista.business.imp.Services;
using o_campista.business.IServices;
using o_campista.repository.imp.Repositories;
using o_campista.repository.IRepositories;
using System.Globalization;
using System.Text;

var culture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CampistaDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.UseNetTopologySuite()
    )
    .EnableSensitiveDataLogging()
    .LogTo(Console.WriteLine)
);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ChatConnectionStore>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});

//services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpClient<StorageService>();
builder.Services.AddScoped<IMapaService, MapaService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPresenteService, PresenteService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<ICheckinService, CheckinService>();
builder.Services.AddScoped<IConquistaService, ConquistaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ICampingAvaliacaoService, CampingAvaliacaoService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ISalaChatService, SalaChatService>();
builder.Services.AddScoped<IUsuarioTrilhaService, UsuarioTrilhaService>();
builder.Services.AddScoped<ITrilhaService, TrilhaService>();
builder.Services.AddScoped<ISocialService, SocialService>();
builder.Services.AddScoped<IFeedService, FeedService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IComentarioPostService, ComentarioPostService>();
builder.Services.AddScoped<IRankingService, RankingService>();
builder.Services.AddScoped<INotificacaoService, NotificacaoService>();
builder.Services.AddScoped<IFavoritoCampingService, FavoritoCampingService>();

//repositories
builder.Services.AddScoped<ICampingFotoRepository, CampingFotoRepository>();
builder.Services.AddScoped<IUsuarioConquistaRepository, UsuarioConquistaRepository>();
builder.Services.AddScoped<ITrilhaRepository, TrilhaRepository>();
builder.Services.AddScoped<IUsuarioPresenteRepository, UsuarioPresenteRepository>();
builder.Services.AddScoped<ICampingRepository, CampingRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPresenteRepository, PresenteRepository>();
builder.Services.AddScoped<ICheckinRepository, CheckinRepository>();
builder.Services.AddScoped<ICampingAvaliacaoRepository, CampingAvaliacaoRepository>();
builder.Services.AddScoped<IMensagemChatRepository, MensagemChatRepository>();
builder.Services.AddScoped<ISalaChatRepository, SalaChatRepository>();
builder.Services.AddScoped<IMensagemSalaChatRepository, MensagemSalaChatRepository>();
builder.Services.AddScoped<IUsuarioTrilhaRepository, UsuarioTrilhaRepository>();
builder.Services.AddScoped<ISocialRepository, SocialRepository>();
builder.Services.AddScoped<IFeedRepository, FeedRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IComentarioPostRepository, ComentarioPostRepository>();
builder.Services.AddScoped<IRankingRepository, RankingRepository>();
builder.Services.AddScoped<INotificacaoRepository, NotificacaoRepository>();
builder.Services.AddScoped<IFavoritoCampingRepository, FavoritoCampingRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("SUPER_SECRET_KEY_O_CAMPISTA_2026")),
            ValidateIssuer = false,
            ValidateAudience = false
        };
        // SignalR WebSocket connections cannot send headers; read token from query string
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken) &&
                    (context.HttpContext.Request.Path.StartsWithSegments("/chatHub") ||
                     context.HttpContext.Request.Path.StartsWithSegments("/notificationHub")))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials();
    });
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.MapHub<ChatNotificationHub>("/notificationHub");
app.Run();
