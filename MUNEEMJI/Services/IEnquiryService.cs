using MUNEEMJI.Models;

namespace MUNEEMJI.Services
{
    public interface IEnquiryService
    {
        Task<List<Enquiry>> GetAllAsync(int companyId, string sectionType);
        Task<Enquiry?> GetByIdAsync(int enquiryId, int companyId);
        Task<int> CreateAsync(Enquiry enquiry);
        Task<bool> UpdateAsync(Enquiry enquiry);
        Task<bool> SoftDeleteAsync(int enquiryId, int companyId);
        Task<bool> UpdateStatusAsync(int enquiryId, int companyId, string status, string? reason);
    }
}
