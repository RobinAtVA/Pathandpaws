using PathAndPaws.Models;
using PathAndPaws.Services;
using Resend;
using Serilog;
using Microsoft.EntityFrameworkCore;
using PathAndPaws.data;

Serilog.Log.Logger = new Serilog.LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddHttpClient();

builder.Services.AddScoped<ResendService>();

builder.Services.AddHttpClient<EmailOctopusService>();

var dataFolder = Path.Combine(
    builder.Environment.ContentRootPath,
    "data");

//Directory.CreateDirectory(dataFolder);

var dbPath = Path.Combine(
    dataFolder,
    "pathandpaws.db");

//logger.LogInformation($"SQLite DB Path: {dbPath}");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    db.Database.Migrate();
}
    
    app.MapPost("/api/contact", async (
    ContactForm form,
    AppDbContext db,
    ResendService resend,
    EmailOctopusService emailOctopus,
    ILogger<Program> logger
) =>
{
    logger.LogInformation("Incoming contact form: {@Form}", form);

    if (string.IsNullOrWhiteSpace(form.Email))
    {
        logger.LogWarning("Contact form missing email");
        return Results.BadRequest("Email required");
    }
 
    if (!form.Email.Contains("@"))
    {
        return Results.BadRequest("Invalid email");
    }

    if (!string.IsNullOrWhiteSpace(form.Website))
    {
        logger.LogWarning("Spam submission detected");
        return Results.BadRequest();
    }

    var lead = new Lead
    {
        Name = form.Name,
        Email = form.Email,
        Phone = form.Phone,
        Company = form.Company,
        Notes = form.Notes
    };



    // ALWAYS SAVE FIRST
    db.Leads.Add(lead);
    await db.SaveChangesAsync();

    logger.LogInformation("Lead stored with ID {Id}", lead.Id);

    // Email notification
    try
    {
        await resend.SendAsync(form);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Resend failed for {Email}", form.Email);
    }

    // EmailOctopus sync
    try
    {
        await emailOctopus.AddContactAsync(form);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "EmailOctopus failed for {Email}", form.Email);
    }

    return Results.Ok(new
    {
        success = true,
        leadId = lead.Id
    });
});

app.MapGet("/api/leads", async (AppDbContext db) =>
{
    var leads = await db.Leads
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync();

    return Results.Ok(leads);
});
;
app.MapGet("/api/health", () => Results.Ok("OK"));
app.MapGet("/api/backup-db", (
    HttpRequest request,
    IConfiguration config,
    IWebHostEnvironment env) =>
{
    var key = request.Query["key"];

    if (key != config["BackupKey"])
    {
        return Results.Unauthorized();
    }

    var dbPath = Path.Combine(
        env.ContentRootPath,
        "Data",
        "pathandpaws.db");

    if (!System.IO.File.Exists(dbPath))
    {
        return Results.NotFound();
    }

    return Results.File(
        dbPath,
        "application/octet-stream",
        "pathandpaws.db");
});

//app.MapGet("/", () => "API is running");

app.MapPost("/api/admin/articles", async (
    Article article,
    AppDbContext db) =>
{
    article.CreatedAt = DateTime.UtcNow;

    db.Articles.Add(article);

    await db.SaveChangesAsync();

    return Results.Ok(article);
});

app.MapGet("/api/admin/articles", async (
    AppDbContext db) =>
{
    return await db.Articles
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync();
});

app.MapPut("/api/admin/articles/{id}", async (
    int id,
    Article updated,
    AppDbContext db) =>
{
    var article = await db.Articles.FindAsync(id);

    if (article == null)
    {
        return Results.NotFound();
    }

    article.Title = updated.Title;
    article.Slug = updated.Slug;
    article.Summary = updated.Summary;
    article.Content = updated.Content;
    article.Published = updated.Published;
    article.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();

    return Results.Ok(article);
});

app.MapDelete("/api/admin/articles/{id}", async (
    int id,
    AppDbContext db) =>
{
    var article = await db.Articles.FindAsync(id);

    if (article == null)
    {
        return Results.NotFound();
    }

    db.Articles.Remove(article);

    await db.SaveChangesAsync();

    return Results.Ok();
});

app.MapGet("/api/articles", async (
    AppDbContext db) =>
{
    return await db.Articles
        .Where(x => x.Published)
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync();
});
app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();
