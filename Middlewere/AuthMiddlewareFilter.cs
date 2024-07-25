using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using RafaelSiteCore.Services.Auth;

namespace RafaelSiteCore.Middlewere
{
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class AuthMiddlewareAttribute : Attribute, IFilterFactory
        {
                public bool IsReusable => false;

                public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
                {
                        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                        var repository = serviceProvider.GetRequiredService<DiscordAuthLogic>();
                        return new AuthMiddlewareFilter(httpContextAccessor,repository);
                }

                private class AuthMiddlewareFilter(IHttpContextAccessor httpContextAccessor, DiscordAuthLogic authLogic) : IActionFilter
                {
                        public void OnActionExecuting(ActionExecutingContext context)
                        {
                                try
                                {
                                        var request = httpContextAccessor.HttpContext!.Request;

                                        if (IsUserAuthorizedByTokenFromHeader(request))
                                                return;

                                        context.Result = new UnauthorizedResult();
                                }
                                catch (Exception)
                                {
                                        context.Result = new UnauthorizedResult();
                                }
                        }

                        private bool IsUserAuthorizedByTokenFromHeader(HttpRequest request)
                        {
                                request.Headers.TryGetValue("Authorization", out var token);

                                if (!string.IsNullOrEmpty(token))
                                {
                                        authLogic.GetUser(token!);

                                        return true;
                                }

                                return false;
                        }

                        public void OnActionExecuted(ActionExecutedContext context)
                        {
                        }
                }
        }
}
