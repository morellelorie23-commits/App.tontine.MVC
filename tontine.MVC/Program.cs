using tontine.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IMembreService, MembreService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5185/");
});

builder.Services.AddHttpClient<ITontineService, TontineService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5185/");
});

builder.Services.AddHttpClient<ICycleService, CycleService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5185/");
});

builder.Services.AddHttpClient<IPosteService, PosteService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5185/");
});

builder.Services.AddHttpClient<IPenaliteService, PenaliteService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5185/");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();