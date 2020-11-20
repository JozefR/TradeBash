using Microsoft.AspNetCore.Mvc;

namespace TradeBash.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : Controller
    {
    }
}
