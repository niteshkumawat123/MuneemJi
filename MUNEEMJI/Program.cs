using Microsoft.AspNetCore.Authentication.Cookies;
using MUNEEMJI.PdfServices;
using MUNEEMJI.Repositories;
using MUNEEMJI.Services;
var builder = WebApplication.CreateBuilder(args);

MUNEEMJI.DbConfig.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";



// Add services to the container.

builder.Services.AddHttpContextAccessor();
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
builder.Services.AddScoped<ICompanyTenancy, CompanyTenancyService>();
builder.Services.AddScoped<IParty, PartyRepository>();
builder.Services.AddScoped<IUser, UserRepository>();
builder.Services.AddScoped<MUNEEMJI.Services.IGstSettingsService, MUNEEMJI.Services.GstSettingsService>();
builder.Services.AddScoped<ISalesInvoicesPdf, SalesInvoicesPdf>();
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
        options.SlidingExpiration = false;
        options.ExpireTimeSpan = TimeSpan.FromDays(365 * 10); 
        options.Cookie.IsEssential = true;
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