using System.Threading.Tasks;

namespace MUNEEMJI.Services
{
    public interface IErrorLogService
    {
        Task LogErrorAsync(string errorMessage, string stackTrace);
    }
}
