using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

namespace ReverseProxyTarget.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        [HttpGet]
        [Route("/proxy-this/item")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new[] { "value1", "value2" };
        }
    }
}
