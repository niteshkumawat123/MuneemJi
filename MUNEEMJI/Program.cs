using MUNEEMJI.Repositories;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPurchaseBillService, PurchaseBillService>();
builder.Services.AddScoped<IBillItemService, BillItemService>();
builder.Services.AddScoped<IGodownService, GodownService>();
builder.Services.AddScoped<ISalesBillService, SalesBillService>();
builder.Services.AddRazorPages();

// Add Session services
builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true; // Security
    options.Cookie.IsEssential = true; // Required for GDPR compliance
});

// Add Authentication services
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login"; // Redirect to login page
        options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect to access denied page
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UsePathBase("/Web"); 

app.UseStaticFiles();
app.UseRouting();

// Add Session middleware (MUST be after UseRouting and before UseAuthorization)
app.UseSession();

app.UseAuthentication(); // Add this before UseAuthorization
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages().RequireAuthorization(); // Enforce authorization globally
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}");
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
});

app.Run();