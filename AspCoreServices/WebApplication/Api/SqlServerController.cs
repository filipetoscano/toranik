using System;
using Microsoft.AspNet.Mvc;

namespace Toranik.Endpoint.Api
{
    [Route( "api/[controller]/[action]" )]
    public class SqlServerController : Controller
    {
        [HttpGet]
        public string StringGet()
        {
            return "";
        }
    }
}
