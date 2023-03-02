using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace Personal_Collection_Manager.Helpers
{
    public class RequestLocalizationCookies : IMiddleware
    {
        public CookieRequestCultureProvider? Provider { get; }

        public RequestLocalizationCookies(IOptions<RequestLocalizationOptions> options)
        {
            Provider = options
                .Value
                .RequestCultureProviders
                .Where(x => x is CookieRequestCultureProvider)
                .Cast<CookieRequestCultureProvider>()
                .FirstOrDefault();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (Provider != null)
            {
                var feature = context.Features.Get<IRequestCultureFeature>();
                if (feature != null)
                {
                    context
                        .Response
                        .Cookies
                        .Append(
                            Provider.CookieName,
                            CookieRequestCultureProvider.MakeCookieValue(feature.RequestCulture)
                        );
                }
            }
            await next(context);
        }
    }
}
