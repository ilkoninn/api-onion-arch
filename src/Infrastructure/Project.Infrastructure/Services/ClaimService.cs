namespace Project.Infrastructure.Implementations.Services;

public class ClaimService(
    IHttpContextAccessor httpContextAccessor) : IClaimService
{
    public string GetUserId()
    {
        var result = httpContextAccessor.HttpContext?.User?.Claims?
            .FirstOrDefault(c => c.Type == "id")?.Value
            ?? string.Empty;
        
        return result;
    }
}