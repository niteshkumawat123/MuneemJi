using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.PdfServices;

namespace MUNEEMJI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfTestController : ControllerBase
    {
        private readonly ISalesInvoicesPdf _salesInvoicesPdf;
        private readonly IWebHostEnvironment _environment;

        public PdfTestController(ISalesInvoicesPdf salesInvoicesPdf, IWebHostEnvironment environment)
        {
            _salesInvoicesPdf = salesInvoicesPdf;
            _environment = environment;
        }

        /// <summary>
        /// GET /api/pdftest/path/{id}
        /// Returns the saved PDF relative path in JSON.
        /// </summary>
        [HttpGet("path/{id}")]
        public async Task<IActionResult> GetPdfPath(int id)
        {
            try
            {
                var relativePath = await _salesInvoicesPdf.GetContractPdfById(id, _environment);

                if (string.IsNullOrEmpty(relativePath))
                    return BadRequest(new { success = false, message = "PDF generation returned empty result." });

                return Ok(new { success = true, pdfPath = relativePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// GET /api/pdftest/download/{id}
        /// Returns the PDF as a downloadable file.
        /// </summary>
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            try
            {
                var relativePath = await _salesInvoicesPdf.GetContractPdfById(id, _environment);

                if (string.IsNullOrEmpty(relativePath))
                    return BadRequest(new { success = false, message = "PDF generation returned empty result." });

                string absolutePath = Path.Combine(_environment.WebRootPath,
                    relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (!System.IO.File.Exists(absolutePath))
                    return NotFound(new { success = false, message = "PDF file not found on server." });

                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(absolutePath);
                return File(fileBytes, "application/pdf", $"Invoice_{id}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message, detail = ex.InnerException?.Message });
            }
        }
    }
}
