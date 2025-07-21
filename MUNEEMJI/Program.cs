using Microsoft.AspNetCore.Authentication.Cookies;
using MUNEEMJI.Repositories;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPurchaseBillService, PurchaseBillService>();
builder.Services.AddScoped<IBillItemService, BillItemService>();
builder.Services.AddScoped<IGodownService, GodownService>();
builder.Services.AddScoped<ISalesBillService, SalesBillService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<ISalesOrderService, SalesOrderService>();
builder.Services.AddScoped<IDeliveryChallanService, DeliveryChallanService>();
builder.Services.AddScoped<IDebitNoteRepository, DebitNoteRepository>();
builder.Services.AddScoped<ICreditNoteRepository, CreditNoteRepository>();
builder.Services.AddScoped<IEstimate_QuotationsRepository, Estimate_QuotationsRepository>();
builder.Services.AddScoped<IOtherIncomeRepository, OtherIncomeRepository>();
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
// Add Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Change this to your login controller/action
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
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