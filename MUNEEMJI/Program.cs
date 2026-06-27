using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MUNEEMJI.Models;
using MUNEEMJI.PdfServices;
using MUNEEMJI.Repositories;
using MUNEEMJI.Services;
var builder = WebApplication.CreateBuilder(args);

MUNEEMJI.DbConfig.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

// Configure Kestrel to allow larger request bodies (applies when running directly on Kestrel)
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; // 50 MB
});

// Configure IIS to allow larger request bodies (applies when hosted behind IIS)
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 50 * 1024 * 1024; // 50 MB
});

// Configure form options for multipart form data uploads
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50 MB
});

// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<MUNEEMJI.Filters.GlobalPermissionFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.MaxDepth = 64;
});
builder.Services.AddScoped<MUNEEMJI.Filters.GlobalPermissionFilter>();
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
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IEnquiryService, EnquiryService>();
builder.Services.AddScoped<IGstTaxService, GstTaxService>();
builder.Services.AddScoped<IDropdownService, DropdownService>();
builder.Services.AddScoped<IErrorLogService, ErrorLogService>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<MUNEEMJI.Services.IGstSettingsService, MUNEEMJI.Services.GstSettingsService>();

// Razorpay configuration
builder.Services.Configure<RazorpaySettings>(builder.Configuration.GetSection("Razorpay"));
builder.Services.AddScoped<IRazorpayService, RazorpayService>();

builder.Services.AddScoped<ISalesInvoicesPdf, SalesInvoicesPdf>();
builder.Services.AddScoped<IEstimationQuotationPdf, EstimationQuotationPdf>();
builder.Services.AddScoped<IPaymentInPdf, PaymentInPdf>();
builder.Services.AddScoped<ISaleOrderPdf, SaleOrderPdf>();
builder.Services.AddScoped<IDeliveryChallanPdf, DeliveryChallanPdf>();
builder.Services.AddScoped<ISaleReturnPdf, SaleReturnPdf>();
builder.Services.AddScoped<ICreditNotePdf, CreditNotePdf>();
builder.Services.AddScoped<IOtherIncomePdf, OtherIncomePdf>();
builder.Services.AddScoped<IPurchaseBillPdf, PurchaseBillPdf>();
builder.Services.AddScoped<IPaymentOutPdf, PaymentOutPdf>();
builder.Services.AddScoped<IExpensePdf, ExpensePdf>();
builder.Services.AddScoped<IPurchaseOrderPdf, PurchaseOrderPdf>();
builder.Services.AddScoped<IPurchaseReturnPdf, PurchaseReturnPdf>();
builder.Services.AddScoped<IDrNotePdf, DrNotePdf>();
builder.Services.AddRazorPages();

// Add Session services
builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(365); // Session never expires on its own
    options.Cookie.HttpOnly = true; // Security
    options.Cookie.IsEssential = true; // Required for GDPR compliance
    options.Cookie.MaxAge = TimeSpan.FromDays(365); // Cookie persists across browser restarts
});

// Add Authentication services
// Add Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Web/Account/Login";
        options.LogoutPath = "/Web/Account/Logout";
        options.SlidingExpiration = false;
        options.ExpireTimeSpan = TimeSpan.FromDays(365 * 10); 
        options.Cookie.IsEssential = true;
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Headers["Accept"].ToString().Contains("application/json") ||
                context.Request.Headers["Content-Type"].ToString().Contains("application/json") ||
                context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"success\":false,\"message\":\"Session expired. Please login again.\"}");
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UsePathBase("/Web");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

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