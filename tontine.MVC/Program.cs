using tontine.MVC.Services;
using Microsoft.Extensions.Logging;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var apiBase = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5185/";

builder.Services.AddHttpClient<IMembreService, MembreService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<ITontineService, TontineService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<ICycleService, CycleService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<IPosteService, PosteService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<IPenaliteService, PenaliteService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<IStatistiqueService, StatistiqueService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<ICotisationService, CotisationService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<IVersementService, VersementService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<IReunionService, ReunionService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<ICompteService, CompteService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<IJournalService, JournalService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<ICycleTontineService, CycleTontineService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<ICycleTontinePenaliteService, CycleTontinePenaliteService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<IMembreCycleTontineService, MembreCycleTontineService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<IMembrePosteCycleService, MembrePosteCycleService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<IPretService, PretService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<IAmendeService, AmendeService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<IGarantService, GarantService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<IAlertePretService, AlertePretService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<IAlerteCotisationService, AlerteCotisationService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<IPaiementMobileService, PaiementMobileService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<IGrandLivreService, GrandLivreService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<ISaisieSeanceService, SaisieSeanceService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddHttpClient<INotificationService, NotificationService>(client =>
    client.BaseAddress = new Uri(apiBase));
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<PdfService>();
builder.Services.AddSingleton<ExcelService>();
QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
