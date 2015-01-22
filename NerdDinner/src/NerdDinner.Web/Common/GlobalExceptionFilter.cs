using System;
using System.Net;
using Microsoft.AspNet.Mvc;

namespace NerdDinner.Web.Common
{
    /// <summary>
    /// Global Exception Filter
    /// Handles all exceptions globally
    /// </summary>
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// On Exception
        /// </summary>
        /// <param name="context">Exception Context</param>
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is NotImplementedException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else if (context.Exception is ArgumentException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Result = new JsonResult(context.Exception.Message);
            }

            context.Result = new ContentResult { Content = Resources.UnknownError };
        }
    }
}
