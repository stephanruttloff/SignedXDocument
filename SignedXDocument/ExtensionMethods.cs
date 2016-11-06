using System.Xml;
using System.Xml.Linq;

namespace Ruttloff.SignedXDocument
{
    internal static class ExtensionMethods
    {
        // http://stackoverflow.com/questions/1508572/converting-xdocument-to-xmldocument-and-vice-versa
        // http://thomasjo.com/blog/2009/08/04/xmldsig-in-the-net-framework.html
        public static XmlDocument ToXmlDocument(this XDocument document)
        {
            var xmlDocument = new XmlDocument { PreserveWhitespace = true };
            xmlDocument.LoadXml(document.ToString(SaveOptions.DisableFormatting));
            return xmlDocument;
        }

        // http://stackoverflow.com/questions/1508572/converting-xdocument-to-xmldocument-and-vice-versa
        public static XDocument ToXDocument(this XmlDocument document)
        {
            using (var reader = new XmlNodeReader(document))
                return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
        }

        // https://gist.github.com/rarous/3150395
        public static XmlElement ToXmlElement(this XElement element)
        {
            var doc = new XmlDocument();
            using (var reader = element.CreateReader())
                doc.Load(reader);
            return doc.DocumentElement;
        }

        public static XElement SetNamespace(this XElement element, XNamespace ns)
        {
            foreach (var xElement in element.DescendantsAndSelf())
                xElement.Name = ns.GetName(xElement.Name.LocalName);

            return element;
        }
    }
}
