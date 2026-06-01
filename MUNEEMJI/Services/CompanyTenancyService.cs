namespace MUNEEMJI.Services
{
     
    public class CompanyTenancyService: ICompanyTenancy
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CompanyTenancyService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCurrentCompanyId()
        {
            var companyIdSession = _httpContextAccessor.HttpContext?.Session.GetString("BusinessId");

            // Fallback: try reading from authentication cookie claim if session is lost
            if (string.IsNullOrEmpty(companyIdSession))
            {
                companyIdSession = _httpContextAccessor.HttpContext?.User
                    ?.FindFirst("CompanyId")?.Value;

                // Re-populate session from claim so subsequent calls are fast
                if (!string.IsNullOrEmpty(companyIdSession))
                {
                    _httpContextAccessor.HttpContext?.Session.SetString("BusinessId", companyIdSession);
                }
            }

            return int.TryParse(companyIdSession, out var companyId)
                ? companyId
                : throw new UnauthorizedAccessException("No company context found");
        }
    }
}
