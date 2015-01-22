using Microsoft.AspNet.Mvc;
using NerdDinner.Web.Common;

namespace NerdDinner.Web.Controllers
{
    /// <summary>
    /// Version Cotroller
    /// </summary>
    [Route("api/[controller]")]
    public class VersionController : Controller
    {
        /// <summary>
        /// Default get method
        /// </summary>
        /// <returns>version of the API</returns>
        public string Get()
        {
            return string.Format(Resources.ApiVersion, "1.0");
        }
    }
}
