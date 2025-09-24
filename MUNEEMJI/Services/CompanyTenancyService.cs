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
            //var companyIdClaim = _httpContextAccessor.HttpContext?.User
            //    ?.FindFirst("CompanyId")?.Value;

            var companyIdSession = _httpContextAccessor.HttpContext?.Session.GetString("BusinessId");


            return int.TryParse(companyIdSession, out var companyId)
                ? companyId
                : throw new UnauthorizedAccessException("No company context found");
        }
    }
}
