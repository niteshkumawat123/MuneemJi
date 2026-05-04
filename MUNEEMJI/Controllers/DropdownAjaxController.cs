using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Services;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    public class DropdownAjaxController : Controller
    {
        private readonly IDropdownService _dropdownService;
        private readonly ICompanyTenancy _companyTenancy;

        public DropdownAjaxController(IDropdownService dropdownService, ICompanyTenancy companyTenancy)
        {
            _dropdownService = dropdownService;
            _companyTenancy = companyTenancy;
        }

        [HttpGet]
        public async Task<IActionResult> Godowns()
        {
            var companyId = _companyTenancy.GetCurrentCompanyId();
            var data = await _dropdownService.GetGodownsAsync(companyId);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var companyId = _companyTenancy.GetCurrentCompanyId();
            var data = await _dropdownService.GetUsersAsync(companyId);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var data = await _dropdownService.GetCategoriesAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> States()
        {
            var data = await _dropdownService.GetStatesAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> BankAccounts()
        {
            var companyId = _companyTenancy.GetCurrentCompanyId();
            var data = await _dropdownService.GetBankAccountsAsync(companyId);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> Units()
        {
            var data = await _dropdownService.GetUnitsAsync();
            return Json(data);
        }
    }
}
