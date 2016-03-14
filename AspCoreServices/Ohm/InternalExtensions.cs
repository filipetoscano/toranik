using System.Xml;

namespace Ohm
{
    public static class InternalExtensions
    {
        public static XmlElement AppendChild( this XmlElement element, string elementName, string @namespace )
        {
            XmlElement elem = element.OwnerDocument.CreateElement( elementName, @namespace );
            element.AppendChild( elem );

            return elem;
        }


        public static XmlAttribute Attr( this XmlElement element, string attributeName )
        {
            XmlAttribute attr = element.OwnerDocument.CreateAttribute( attributeName );
            element.Attributes.Append( attr );

            return attr;
        }
    }
}
