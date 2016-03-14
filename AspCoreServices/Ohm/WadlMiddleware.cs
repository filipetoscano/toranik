using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace Ohm
{
    public class WadlMiddleware
    {
        private readonly RequestDelegate _next;

        private const string XSD = "http://www.w3.org/2001/XMLSchema";
        private const string WADL = "http://wadl.dev.java.net/2009/02";


        public WadlMiddleware( RequestDelegate next )
        {
            _next = next;
        }


        public Task Invoke( HttpContext httpContext, ILibraryManager libraryManager )
        {
            #region Validations

            if ( httpContext == null )
                throw new ArgumentNullException( "httpContext" );

            if ( libraryManager == null )
                throw new ArgumentNullException( "libraryManager" );

            #endregion

            if ( httpContext.Request.Path == "/api.wadl" )
            {
                try
                {
                    return HandleRequest( httpContext, libraryManager );
                }
                catch ( Exception )
                {
                    httpContext.Response.StatusCode = 500;
                    return httpContext.Response.WriteAsync( "" );
                }
            }

            return _next( httpContext );
        }


        private static Task HandleRequest( HttpContext httpContext, ILibraryManager libraryManager )
        {
            /*
             * Go through all of the currently loaded libraries/assemblies
             * looking for all implementations of WebAPI controller. No
             * point looking System/Microsoft libraries.
             */
            HashSet<Type> controllers = new HashSet<Type>();
            Type controllerBase = typeof( Controller );

            foreach ( var library in libraryManager.GetLibraries() )
            {
                if ( library.Name.StartsWith( "Microsoft.", StringComparison.Ordinal ) == true )
                    continue;

                if ( library.Name.StartsWith( "System.", StringComparison.Ordinal ) == true )
                    continue;

                if ( library.Name.StartsWith( "fx/Microsoft.", StringComparison.Ordinal ) == true )
                    continue;

                if ( library.Name.StartsWith( "fx/System", StringComparison.Ordinal ) == true )
                    continue;

                if ( library.Name == "fx/mscorlib" || library.Name == "Newtonsoft.Json" )
                    continue;

                foreach ( var assemblyRef in library.Assemblies )
                {
                    Assembly ass = Assembly.Load( assemblyRef );

                    foreach ( var t in ass.DefinedTypes )
                    {
                        Type tt = t.AsType();

                        if ( controllerBase.IsAssignableFrom( tt ) == false )
                            continue;

                        RouteAttribute ra = t.GetCustomAttribute<RouteAttribute>();

                        if ( ra == null )
                            continue;

                        controllers.Add( tt );
                    }
                }
            }


            /*
             * Collect all of the types explicitly referenced in the API.
             * Recursively iterate through these, thereby obtaining a list
             * of all the types used by the API.
             */
            HashSet<Type> explicitTypes = new HashSet<Type>();
            HashSet<Type> types = new HashSet<Type>();

            foreach ( Type ct in controllers )
            {
                foreach ( MethodInfo m in ct.GetMethods() )
                {
                    if ( IsRestMethod( m ) == false )
                        continue;

                    foreach ( var p in m.GetParameters() )
                        explicitTypes.Add( ToNormalType( p.ParameterType ) );

                    if ( m.ReturnType != null )
                        explicitTypes.Add( ToNormalType( m.ReturnType ) );
                }
            }

            Func<Type, bool> walk = null;
            walk = ( Type type ) =>
            {
                if ( types.Contains( type ) == true )
                    return false;

                if ( IsCustomType( type ) == false )
                    return false;

                types.Add( type );

                foreach ( var f in type.GetFields( BindingFlags.Public | BindingFlags.Instance ) )
                    walk( ToNormalType( f.FieldType ) );

                foreach ( var p in type.GetProperties( BindingFlags.Public | BindingFlags.Instance ) )
                    walk( ToNormalType( p.PropertyType ) );

                return true;
            };

            foreach ( Type t in explicitTypes )
            {
                walk( t );
            }


            /*
             *
             */
            XmlNamespaceManager manager = new XmlNamespaceManager( new NameTable() );
            manager.AddNamespace( "wadl", WADL );
            manager.AddNamespace( "xsd", XSD );

            XmlDocument doc = new XmlDocument();
            doc.LoadXml( @"<application xmlns='http://wadl.dev.java.net/2009/02' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:ns='urn:toranik'>
    <grammars>
        <xsd:schema elementFormDefault='qualified' targetNamespace='urn:toranik'>
        </xsd:schema>
    </grammars>
    <resources base='http://localhost:6003/api' />
</application>" );

            XmlElement schema = (XmlElement) doc.DocumentElement.ChildNodes[ 0 ].ChildNodes[ 0 ];

            foreach ( Type t in types )
            {
                EmitSchema( schema, t );
                EmitArray( schema, t );
            }


            /*
             *
             */
            XmlElement resources = (XmlElement) doc.DocumentElement.ChildNodes[ 1 ];

            foreach ( var c in controllers )
            {
                RouteAttribute route = c.GetTypeInfo().GetCustomAttribute<RouteAttribute>();

                if ( route.Template == "api/[controller]/[action]" )
                {
                    XmlElement controller = resources.AppendChild( "resource", WADL );
                    controller.SetAttribute( "path", "/" + ToControllerName( c ) );

                    foreach ( var m in c.GetMethods() )
                    {
                        if ( IsRestMethod( m ) == false )
                            continue;

                        var resource = controller.AppendChild( "resource", WADL );
                        resource.SetAttribute( "path", "/" + m.Name );

                        var method = resource.AppendChild( "method", WADL );
                        method.SetAttribute( "name", ToRestMethod( m ) );

                        if ( m.GetParameters().Length > 0 )
                        {
                            var request = method.AppendChild( "request", WADL );

                            foreach ( var p in m.GetParameters() )
                            {
                                FromBodyAttribute fromBody = p.GetCustomAttribute<FromBodyAttribute>();
                                FromQueryAttribute fromQuery = p.GetCustomAttribute<FromQueryAttribute>();

                                if ( fromQuery != null )
                                {
                                    var param = request.AppendChild( "param", WADL );
                                    param.SetAttribute( "name", p.Name );
                                    param.SetAttribute( "style", "query" );
                                    param.SetAttribute( "type", ToSchemaType( p.ParameterType ) );

                                    if ( p.HasDefaultValue == false )
                                        param.SetAttribute( "required", "true" );
                                    else
                                        param.SetAttribute( "default", p.DefaultValue.ToString() );
                                }

                                if ( fromBody != null )
                                {
                                    var repr = request.AppendChild( "representation", WADL );
                                    repr.SetAttribute( "mediaType", "application/json" );
                                    repr.SetAttribute( "element", ToSchemaType( p.ParameterType ) );
                                }
                            }
                        }

                        if ( m.ReturnType != null )
                        {
                            var response = method.AppendChild( "response", WADL );
                            response.SetAttribute( "status", "200" );

                            var repr = response.AppendChild( "representation", WADL );
                            repr.SetAttribute( "mediaType", "application/json" );
                            repr.SetAttribute( "element", ToSchemaType( m.ReturnType ) );
                        }
                    }
                }
            }


            /*
             *
             */
            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "application/xml";

            return httpContext.Response.WriteAsync( doc.OuterXml );
        }


        private static string ToControllerName( Type controller )
        {
            #region Validations

            if ( controller == null )
                throw new ArgumentNullException( "" );

            #endregion

            string name = controller.Name;

            if ( name.EndsWith( "Controller" ) == true )
                return name.Substring( 0, name.Length - "Controller".Length );

            return name;
        }


        private static void EmitSchema( XmlElement schema, Type type )
        {
            #region Validations

            if ( schema == null )
                throw new ArgumentNullException( "schema" );

            if ( type == null )
                throw new ArgumentNullException( "type" );

            #endregion

            var complexType = schema.AppendChild( "xsd:complexType", XSD );
            complexType.SetAttribute( "name", type.Name );

            var sequence = complexType.AppendChild( "xsd:sequence", XSD );

            foreach ( var f in type.GetFields( BindingFlags.Public | BindingFlags.Instance ) )
            {
                if ( f.IsPrivate == true )
                    continue;

                EmitMember( sequence, f.Name, f.FieldType );
            }

            foreach ( var p in type.GetProperties( BindingFlags.Public | BindingFlags.Instance ) )
            {
                EmitMember( sequence, p.Name, p.PropertyType );
            }
        }


        private static void EmitMember( XmlElement sequence, string name, Type type )
        {
            #region Validations

            if ( sequence == null )
                throw new ArgumentNullException( "sequence" );

            if ( name == null )
                throw new ArgumentNullException( "name" );

            if ( type == null )
                throw new ArgumentNullException( "type" );

            #endregion

            string xsdType = "";
            string minOccurs = "1";
            string maxOccurs = "1";
            string nillable = "false";

            Type nt = Nullable.GetUnderlyingType( type );

            if ( nt != null )
            {
                minOccurs = "0";
                xsdType = ToNativeSchemaType( nt );
            }
            else if ( type.IsArray == true || type.GetTypeInfo().IsGenericType == true )
            {
                Type et;

                if ( type.IsArray == true )
                    et = type.GetElementType();
                else
                    et = type.GetType().GetGenericArguments()[ 0 ];

                if ( IsCustomType( et ) == true )
                {
                    minOccurs = "0";
                    xsdType = "ArrayOf" + et.Name;
                }
                else
                {
                    minOccurs = "0";
                    maxOccurs = "unbounded";
                    xsdType = ToNativeSchemaType( et );
                }
            }
            else
            {
                if ( type.GetTypeInfo().GetCustomAttribute<OptionalAttribute>() != null )
                    minOccurs = "0";

                if ( IsCustomType( type ) == true )
                    xsdType = type.Name;
                else
                    xsdType = ToNativeSchemaType( type );
            }

            var element = sequence.AppendChild( "xsd:element", XSD );
            element.SetAttribute( "name", name );
            element.SetAttribute( "type", xsdType );
            element.SetAttribute( "minOccurs", minOccurs );
            element.SetAttribute( "maxOccurs", maxOccurs );
            element.SetAttribute( "nillable", nillable );
        }


        private static void EmitArray( XmlElement schema, Type type )
        {
            #region Validations

            if ( schema == null )
                throw new ArgumentNullException( "schema" );

            if ( type == null )
                throw new ArgumentNullException( "type" );

            #endregion

            var complexType = schema.AppendChild( "xsd:complexType", XSD );
            complexType.SetAttribute( "name", "ArrayOf" + type.Name );

            var sequence = complexType.AppendChild( "xsd:sequence", XSD );

            var element = sequence.AppendChild( "xsd:element", XSD );
            element.SetAttribute( "name", type.Name );
            element.SetAttribute( "type", type.Name );
            element.SetAttribute( "minOccurs", "0" );
            element.SetAttribute( "maxOccurs", "unbounded" );
            element.SetAttribute( "nillable", "true" );
        }


        private static string ToSchemaType( Type type )
        {
            #region Validations

            if ( type == null )
                throw new ArgumentNullException( "type" );

            #endregion

            string xsdType;

            if ( type.IsArray == true || type.GetTypeInfo().IsGenericType == true )
            {
                Type et;

                if ( type.IsArray == true )
                    et = type.GetElementType();
                else
                    et = type.GetType().GetGenericArguments()[ 0 ];

                if ( IsCustomType( et ) == true )
                    xsdType = "ns:ArrayOf" + et.Name;
                else
                    xsdType = ToNativeSchemaType( et );
            }
            else
            {
                if ( IsCustomType( type ) == true )
                    xsdType = "ns:" + type.Name;
                else
                    xsdType = ToNativeSchemaType( type );
            }

            return xsdType;
        }


        private static string ToNativeSchemaType( Type type )
        {
            #region Validations

            if ( type == null )
                throw new ArgumentNullException( "type" );

            #endregion

            string v;

            switch ( type.FullName )
            {
                case "System.Boolean":
                    v = "xsd:boolean";
                    break;

                case "System.Int16":
                    v = "xsd:short";
                    break;

                case "System.Int32":
                    v = "xsd:int";
                    break;

                case "System.Int64":
                    v = "xsd:long";
                    break;

                case "System.Single":
                    v = "xsd:float";
                    break;

                case "System.Double":
                    v = "xsd:double";
                    break;

                case "System.Decimal":
                    v = "xsd:decimal";
                    break;

                /*
                 * TODO: What about date/time? :/
                 */
                case "System.DateTime":
                    v = "xsd:dateTime";
                    break;

                case "System.String":
                    v = "xsd:string";
                    break;

                case "System.Uri":
                    v = "xsd:string";
                    break;

                default:
                    v = "xsd:any";
                    break;
            }

            return v;
        }


        private static Type ToNormalType( Type parameterType )
        {
            #region Validations

            if ( parameterType == null )
                throw new ArgumentNullException( "parameterType" );

            #endregion

            Type nt = Nullable.GetUnderlyingType( parameterType );

            if ( nt != null )
            {
                return nt;
            }
            else if ( parameterType.IsArray == true )
            {
                return parameterType.GetElementType();
            }
            else if ( parameterType.GetTypeInfo().IsGenericType == true )
            {
                return parameterType.GetGenericArguments()[ 0 ];
            }
            else
            {
                return parameterType;
            }
        }


        private static bool IsCustomType( Type type )
        {
            #region Validations

            if ( type == null )
                throw new ArgumentNullException( "type" );

            #endregion

            if ( type == typeof( string ) )
                return false;

            if ( type == typeof( Uri ) )
                return false;

            if ( type.GetTypeInfo().IsEnum == true )
                return true;

            return type.GetTypeInfo().IsClass;
        }


        private static bool IsRestMethod( MethodInfo method )
        {
            #region Validations

            if ( method == null )
                throw new ArgumentNullException( "method" );

            #endregion

            if ( method.GetCustomAttribute<HttpGetAttribute>() != null )
                return true;

            if ( method.GetCustomAttribute<HttpPostAttribute>() != null )
                return true;

            if ( method.GetCustomAttribute<HttpDeleteAttribute>() != null )
                return true;

            if ( method.GetCustomAttribute<HttpPutAttribute>() != null )
                return true;

            if ( method.GetCustomAttribute<HttpHeadAttribute>() != null )
                return true;

            if ( method.GetCustomAttribute<HttpPatchAttribute>() != null )
                return true;

            return false;
        }


        private static string ToRestMethod( MethodInfo method )
        {
            #region Validations

            if ( method == null )
                throw new ArgumentNullException( "method" );

            #endregion

            if ( method.GetCustomAttribute<HttpGetAttribute>() != null )
                return "GET";

            if ( method.GetCustomAttribute<HttpPostAttribute>() != null )
                return "POST";

            if ( method.GetCustomAttribute<HttpDeleteAttribute>() != null )
                return "DELETE";

            if ( method.GetCustomAttribute<HttpPutAttribute>() != null )
                return "PUT";

            if ( method.GetCustomAttribute<HttpHeadAttribute>() != null )
                return "HEAD";

            if ( method.GetCustomAttribute<HttpPatchAttribute>() != null )
                return "PATCH";

            return null;
        }
    }
}
