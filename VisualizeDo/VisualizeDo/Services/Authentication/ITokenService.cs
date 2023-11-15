using Microsoft.AspNetCore.Identity;

namespace VisualizeDo.Services.Authentication;

public interface ITokenService
{
    public string CreateToken(IdentityUser user, string role);
}