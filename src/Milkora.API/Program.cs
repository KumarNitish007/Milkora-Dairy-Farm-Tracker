using System.IO.Compression;
using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.SqlServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Milkora.API.Filters;
using Milkora.API.Middleware;
using Milkora.Application;
using Milkora.Application.Common;
using Milkora.Infrastructure;
using Milkora.Sync;

var builder = WebApplication.CreateBuilder(args);

// ---- CORS (Angular client) --------------------------------------------------
const string CorsPolicy = "MilkoraCors";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                     ?? new[] { "http://localhost:4200" };
builder.Services.AddCors(o => o.AddPolicy(CorsPolicy, p =>
    p.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

// ---- Layers (Dependency Injection) -----------------------------------------
builder.Services.AddApplication();      // services + validators
builder.Services.AddInfrastructure();   // repositories + connection factory

// ---- Dotmim.Sync server (SQLite clients <-> central MS SQL) -----------------
var syncConnectionString = builder.Configuration.GetConnectionString("MilkoraDb")!;
builder.Services.AddSyncServer(
    new SqlSyncProvider(syncConnectionString),
    Milkora.Sync.SyncTables.CreateSetup(),
    new SyncOptions { ConflictResolutionPolicy = ConflictResolutionPolicy.ServerWins });

// Dotmim.Sync's web server keeps per-session batch state between requests.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    o.IdleTimeout = TimeSpan.FromMinutes(30);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});

// ---- MVC + global validation filter ----------------------------------------
builder.Services.AddControllers(o => o.Filters.Add<ValidationFilter>());

// Model-binding failures (bad JSON / wrong types) also use the ApiResponse shape.
builder.Services.Configure<ApiBehaviorOptions>(o =>
{
    o.InvalidModelStateResponseFactory = ctx =>
    {
        var errors = ctx.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(e => e.Key, e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray());
        return new BadRequestObjectResult(ApiResponse.Fail("Invalid request.", errors));
    };
});

// ---- Performance: response compression --------------------------------------
builder.Services.AddResponseCompression(o =>
{
    o.EnableForHttps = true;
    o.Providers.Add<BrotliCompressionProvider>();
    o.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ---- Pipeline ---------------------------------------------------------------
app.UseResponseCompression();
app.UseMiddleware<ExceptionHandlingMiddleware>();   // wraps everything downstream

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Serve the Angular app from wwwroot (single origin — enables the PWA / mobile install).
// Populate wwwroot at deploy time: copy Milkora.Client/dist/milkora.client/browser here.
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors(CorsPolicy);
app.UseSession();          // required by the Dotmim.Sync web server
app.MapControllers();

// SPA deep links (e.g. /animals) fall back to the Angular entry point.
app.MapFallbackToFile("index.html");

app.Run();

// Exposes the implicit Program class to the integration test project (WebApplicationFactory<Program>).
public partial class Program { }
