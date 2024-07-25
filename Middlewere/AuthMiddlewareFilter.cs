using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace RafaelSiteCore.Middlewere
{
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class AuthMiddlewareAttribute : Attribute, IFilterFactory
        {
                public bool IsReusable => false;

                public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
                {
                        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                        return new AuthMiddlewareFilter(httpContextAccessor);
                }

                private class AuthMiddlewareFilter(IHttpContextAccessor httpContextAccessor) : IActionFilter
                {
                        public void OnActionExecuting(ActionExecutingContext context)
                        {
                                try
                                {
                                        var request = httpContextAccessor.HttpContext!.Request;

                                        //if (IsUserAuthorizedByTokenFromHeader(request))
                                        //        return;

                                        context.Result = new UnauthorizedResult();
                                }
                                catch (Exception e)
                                {
                                        context.Result = new UnauthorizedResult();
                                }
                        }

                        //private bool IsUserAuthorizedByTokenFromHeader(HttpRequest request)
                        //{
                        //        request.Headers.TryGetValue("Authorization", out var token);

                        //        if (!string.IsNullOrEmpty(token))
                        //        {
                        //                repository.GetUserByAuthToken(token!);

                        //                return true;
                        //        }

                        //        return false;
                        //}

                        public void OnActionExecuted(ActionExecutedContext context)
                        {
                        }
                }
        }
}
