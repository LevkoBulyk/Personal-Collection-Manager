using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Personal_Collection_Manager.Data.DataBaseModels;

public class CustomSignInManager : SignInManager<ApplicationUser>
{
    public CustomSignInManager(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
        IOptions<IdentityOptions> options,
        ILogger<SignInManager<ApplicationUser>> logger,
        IAuthenticationSchemeProvider provider,
        IUserConfirmation<ApplicationUser> confirmation)
        : base(userManager, contextAccessor, claimsFactory, options, logger, provider, confirmation)
    { }

    public override async Task<bool> CanSignInAsync(ApplicationUser user)
    {
        if (user.Blocked)
        {
            return false;
        }
        return await base.CanSignInAsync(user);
    }
}
