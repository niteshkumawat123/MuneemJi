using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.PdfServices;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    [Route("Web/[controller]")]
    public class PdfDownloadController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ISalesInvoicesPdf _salesPdf;
        private readonly IEstimationQuotationPdf _estimationPdf;
        private readonly IPaymentInPdf _paymentInPdf;
        private readonly ISaleOrderPdf _saleOrderPdf;
        private readonly IDeliveryChallanPdf _deliveryChallanPdf;
        private readonly ISaleReturnPdf _saleReturnPdf;
        private readonly ICreditNotePdf _creditNotePdf;
        private readonly IOtherIncomePdf _otherIncomePdf;
        private readonly IPurchaseBillPdf _purchaseBillPdf;
        private readonly IPaymentOutPdf _paymentOutPdf;
        private readonly IExpensePdf _expensePdf;
        private readonly IPurchaseOrderPdf _purchaseOrderPdf;
        private readonly IPurchaseReturnPdf _purchaseReturnPdf;
        private readonly IDrNotePdf _drNotePdf;

        public PdfDownloadController(
            IWebHostEnvironment env,
            ISalesInvoicesPdf salesPdf,
            IEstimationQuotationPdf estimationPdf,
            IPaymentInPdf paymentInPdf,
            ISaleOrderPdf saleOrderPdf,
            IDeliveryChallanPdf deliveryChallanPdf,
            ISaleReturnPdf saleReturnPdf,
            ICreditNotePdf creditNotePdf,
            IOtherIncomePdf otherIncomePdf,
            IPurchaseBillPdf purchaseBillPdf,
            IPaymentOutPdf paymentOutPdf,
            IExpensePdf expensePdf,
            IPurchaseOrderPdf purchaseOrderPdf,
            IPurchaseReturnPdf purchaseReturnPdf,
            IDrNotePdf drNotePdf)
        {
            _env = env;
            _salesPdf = salesPdf;
            _estimationPdf = estimationPdf;
            _paymentInPdf = paymentInPdf;
            _saleOrderPdf = saleOrderPdf;
            _deliveryChallanPdf = deliveryChallanPdf;
            _saleReturnPdf = saleReturnPdf;
            _creditNotePdf = creditNotePdf;
            _otherIncomePdf = otherIncomePdf;
            _purchaseBillPdf = purchaseBillPdf;
            _paymentOutPdf = paymentOutPdf;
            _expensePdf = expensePdf;
            _purchaseOrderPdf = purchaseOrderPdf;
            _purchaseReturnPdf = purchaseReturnPdf;
            _drNotePdf = drNotePdf;
        }

        private async Task<IActionResult> DownloadHelper(string relativePath, string downloadName)
        {
            if (string.IsNullOrEmpty(relativePath))
                return NotFound("PDF could not be generated.");
            string absolutePath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(absolutePath))
                return NotFound("PDF file not found.");
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(absolutePath);
            return File(fileBytes, "application/pdf", downloadName);
        }

        [HttpGet("SalesInvoice/{id}")]
        public async Task<IActionResult> SalesInvoice(int id)
        {
            try { return await DownloadHelper(await _salesPdf.GetContractPdfById(id, _env), $"Invoice_{id}.pdf"); }
            catch { return StatusCode(500, "Error generating PDF."); }
        }

        [HttpGet("Estimation/{id}")]
        public async Task<IActionResult> Estimation(int id)
        {
            try { return await DownloadHelper(await _estimationPdf.GetEstimationPdfById(id, _env), $"Estimation_{id}.pdf"); }
            catch { return StatusCode(500, "Error generating PDF."); }
        }

        [HttpGet("PaymentIn/{id}")]
        public async Task<IActionResult> PaymentIn(int id)
        {
            try { return await DownloadHelper(await _paymentInPdf.GetPaymentInPdfById(id, _env), $"PaymentIn_{id}.pdf"); }
            catch { return StatusCode(500, "Error generating PDF."); }
        }

        [HttpGet("SaleOrder/{id}")]
        public async Task<IActionResult> SaleOrder(int id)
        {
            try { return await DownloadHelper(await _saleOrderPdf.GetSaleOrderPdfById(id, _env), $"SaleOrder_{id}.pdf"); }
            catch { return StatusCode(500, "Error generating PDF."); }
        }

        [HttpGet("DeliveryChallan/{id}")]
        public async Task<IActionResult> DeliveryChallan(int id)
        {
            try { return await DownloadHelper(await _deliveryChallanPdf.GetDeliveryChallanPdfById(id, _env), $"DeliveryChallan_{id}.pdf"); }
            catch { return StatusCode(500, "Error generating PDF."); }
        }

        [HttpGet("SaleReturn/{id}")]
        public async Task<IActionResult> SaleReturn(int id)
        {
            try { return await DownloadHelper(await _saleReturnPdf.GetSaleReturnPdfById(id, _env), $"SaleReturn_{id}.pdf"); }
            catch { return StatusCode(500, "Error generating PDF."); }
        }

        [HttpGet("CreditNote/{id}")]
        public async Task<IActionResult> CreditNote(int id)
        {
            try { return await DownloadHelper(await _creditNotePdf.GetCreditNotePdfById(id, _env), $"CreditNote_{id}.pdf"); }
            catch { return StatusCode(500, "Error generating PDF."); }
        }

        [HttpGet("OtherIncome/{id}")]
        public async Task<IActionResult> OtherIncome(int id)
        {
            try { return await DownloadHelper(await _otherIncomePdf.GetOtherIncomePdfById(id, _env), $"OtherIncome_{id}.pdf"); }
            catch { return StatusCode(500, "Error generating PDF."); }
        }

        [HttpGet("PurchaseBill/{id}")]
        public async Task<IActionResult> PurchaseBill(int id)
        {
            try { return await DownloadHelper(await _purchaseBillPdf.GetPurchaseBillPdfById(id, _env), $"PurchaseBill_{id}.pdf"); }
            catch { return StatusCode(500, "Error generating PDF."); }
        }

        [HttpGet("PaymentOut/{id}")]
        public async Task<IActionResult> PaymentOut(int id)
        {
            try { return await DownloadHelper(await _paymentOutPdf.GetPaymentOutPdfById(id, _env), $"PaymentOut_{id}.pdf"); }
            catch { return StatusCode(500, "Error generating PDF."); }
        }

        [HttpGet("Expense/{id}")]
        public async Task<IActionResult> Expense(int id)
        {
            try { return await DownloadHelper(await _expensePdf.GetExpensePdfById(id, _env), $"Expense_{id}.pdf"); }
            catch { return StatusCode(500, "Error generating PDF."); }
        }

        [HttpGet("PurchaseOrder/{id}")]
        public async Task<IActionResult> PurchaseOrder(int id)
        {
            try { return await DownloadHelper(await _purchaseOrderPdf.GetPurchaseOrderPdfById(id, _env), $"PurchaseOrder_{id}.pdf"); }
            catch { return StatusCode(500, "Error generating PDF."); }
        }

        [HttpGet("PurchaseReturn/{id}")]
        public async Task<IActionResult> PurchaseReturn(int id)
        {
            try { return await DownloadHelper(await _purchaseReturnPdf.GetPurchaseReturnPdfById(id, _env), $"PurchaseReturn_{id}.pdf"); }
            catch { return StatusCode(500, "Error generating PDF."); }
        }

        [HttpGet("DebitNote/{id}")]
        public async Task<IActionResult> DebitNote(int id)
        {
            try { return await DownloadHelper(await _drNotePdf.GetDrNotePdfById(id, _env), $"DebitNote_{id}.pdf"); }
            catch { return StatusCode(500, "Error generating PDF."); }
        }
    }
}
