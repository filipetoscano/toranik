using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;

namespace Ohm
{
    public static class PublicExtensions
    {
        public static IApplicationBuilder UseWadl( this IApplicationBuilder app )
        {
            app.UseMiddleware<WadlMiddleware>();

            return app;
        }
    }
}
