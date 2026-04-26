using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Services;
using MUNEEMJI.Repositories;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    public class EnquiryController : Controller
    {
        private readonly IEnquiryService _enquiryService;
        private readonly ICompanyTenancy _companyTenancy;
        private readonly IUser _userService;

        public EnquiryController(IEnquiryService enquiryService, ICompanyTenancy companyTenancy, IUser userService)
        {
            _enquiryService = enquiryService;
            _companyTenancy = companyTenancy;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var companyId = _companyTenancy.GetCurrentCompanyId();
            var enquiries = await _enquiryService.GetAllAsync(companyId, "enquiry");
            var users = await _userService.GetUserDropdown(companyId);
            ViewBag.Users = users;
            return View(enquiries);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EnquiryViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Please fill all required fields." });

            try
            {
                var companyId = _companyTenancy.GetCurrentCompanyId();
                var enquiry = new Enquiry
                {
                    CompanyId = companyId,
                    CustomerName = model.CustomerName,
                    CustomerPhone = model.CustomerPhone,
                    CustomerEmail = model.CustomerEmail,
                    Subject = model.Subject,
                    Message = model.Message,
                    EnquirySource = model.EnquirySource,
                    AssignedTo = model.AssignedTo,
                    SectionType = "enquiry",
                    Status = "new"
                };

                var id = await _enquiryService.CreateAsync(enquiry);
                return Json(new { success = true, message = "Enquiry created successfully!", enquiryId = id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var companyId = _companyTenancy.GetCurrentCompanyId();
            var enquiry = await _enquiryService.GetByIdAsync(id, companyId);
            if (enquiry == null) return Json(new { success = false, message = "Not found." });
            return Json(new { success = true, data = enquiry });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] EnquiryViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Please fill all required fields." });

            try
            {
                var companyId = _companyTenancy.GetCurrentCompanyId();
                var enquiry = new Enquiry
                {
                    EnquiryId = model.EnquiryId,
                    CompanyId = companyId,
                    CustomerName = model.CustomerName,
                    CustomerPhone = model.CustomerPhone,
                    CustomerEmail = model.CustomerEmail,
                    Subject = model.Subject,
                    Message = model.Message,
                    EnquirySource = model.EnquirySource,
                    AssignedTo = model.AssignedTo
                };

                var ok = await _enquiryService.UpdateAsync(enquiry);
                return Json(new { success = ok, message = ok ? "Enquiry updated!" : "Update failed." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var companyId = _companyTenancy.GetCurrentCompanyId();
                var ok = await _enquiryService.SoftDeleteAsync(id, companyId);
                return Json(new { success = ok, message = ok ? "Enquiry deleted!" : "Delete failed." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] EnquiryStatusUpdateDto dto)
        {
            if ((dto.Status == "unverified" || dto.Status == "closed") && string.IsNullOrWhiteSpace(dto.Reason))
                return Json(new { success = false, message = "Reason is required for this status." });

            try
            {
                var companyId = _companyTenancy.GetCurrentCompanyId();
                var ok = await _enquiryService.UpdateStatusAsync(dto.EnquiryId, companyId, dto.Status, dto.Reason);
                return Json(new { success = ok, message = ok ? "Status updated!" : "Update failed." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
