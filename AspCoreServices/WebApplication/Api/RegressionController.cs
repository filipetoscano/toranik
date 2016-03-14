using System;
using Microsoft.AspNet.Mvc;
using System.Globalization;

namespace Toranik.Endpoint.Api
{
    [Route( "api/[controller]/[action]" )]
    public class RegressionController : Controller
    {
        [HttpGet]
        public Native Native()
        {
            return new Native()
            {
                Boolean = true,
                Decimal = 1.23m,
                Double = 2.34,
                Float = 3.45f,
                Integer = 4,
                Long = 5,
                Short = 6,
                DateTime = DateTime.UtcNow,
                String = "hello world",
                Uri = new Uri( "http://github.com" )
            };
        }


        [HttpGet]
        public NativeOptional NativeOptional()
        {
            return new NativeOptional();
        }


        [HttpGet]
        public Enumerates Enumerates()
        {
            return new Enumerates()
            {
                Normal = EnumerateNormal.One,
                NormalArray = new EnumerateNormal[] { EnumerateNormal.One, EnumerateNormal.Two, EnumerateNormal.Three },
                Flags = EnumerateFlags.First | EnumerateFlags.Second,
                FlagsArray = new EnumerateFlags[] { EnumerateFlags.None, EnumerateFlags.First | EnumerateFlags.Third, EnumerateFlags.Second | EnumerateFlags.Third }
            };
        }


        [HttpGet]
        public PropertyVsField PropertyVsField()
        {
            return new PropertyVsField()
            {
                Field = "I'm a pield",
                Property = "I'm a property"
            };
        }


        [HttpGet]
        public NativeArray NativeArray()
        {
            return new NativeArray()
            {
                Boolean = new bool[] { true, false },
                Short = new short[] { 1, 2, 3 },
                Integer = new int[] { 4, 5, 6 },
                Long = new long[] { 7, 8, 9 },
                Float = new float[] { 10.1f, 11.1f, 12.2f },
                Double = new double[] { 13.4, 14.5, 15.5 },
                Decimal = new decimal[] { 16.6m, 17.7m, 18.8m },
                DateTime = new DateTime[] { DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow },
                String = new string[] { "one", "two", "three" },
                Uri = new Uri[] { new Uri( "http://github.com" ), new Uri( "http://stackoverflow.com" ) }
            };
        }


        [HttpGet]
        public Recursive Recursive()
        {
            return new Recursive()
            {
                Recurse = new Recursive()
                {
                    Recurse = new Recursive()
                },
                RecurseArray = new Recursive[] { }
            };
        }


        [HttpGet]
        public Depth0 Depth()
        {
            return new Depth0()
            {
                Name = "one",
                Depth = new Depth1()
                {
                    Name = "two",
                    Depth = new Depth2()
                    {
                        Name = "three"
                    }
                }
            };
        }


        [HttpGet]
        public Depth0 Query1( [FromQuery] int userId )
        {
            return new Depth0()
            {
                Name = userId.ToString( CultureInfo.InvariantCulture )
            };
        }


        [HttpGet]
        public Depth0 QueryN( [FromQuery] int userId, [FromQuery] int anotherId = 10 )
        {
            return new Depth0()
            {
                Name = userId.ToString( CultureInfo.InvariantCulture ),
                Depth = new Depth1()
                {
                    Name = anotherId.ToString( CultureInfo.InvariantCulture )
                }
            };
        }


        [HttpPost]
        public Depth0 Body( [FromBody] Depth0 depth )
        {
            depth.Name = depth.Name + " roundtrip";

            return depth;
        }


        [HttpPost]
        public Depth0 BodyAndQuery( [FromQuery] int userId, [FromBody] Depth0 depth )
        {
            depth.Name = depth.Name + " " + userId.ToString( CultureInfo.InvariantCulture );

            return depth;
        }
    }
}
