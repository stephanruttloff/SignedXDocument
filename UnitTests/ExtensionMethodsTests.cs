using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ruttloff.SignedXDocument.UnitTests
{
    [TestClass]
    public class ExtensionMethodsTests
    {
        [TestMethod]
        public void ToXDocument()
        {
            const string xml =
                @"<!--root comment-->
                <root>
                    <child>content</child>
                </root>";

            var xmlDocument = new XmlDocument { PreserveWhitespace = true };
            xmlDocument.LoadXml(xml);
            var xDocument = xmlDocument.ToXDocument();

            var xmlDocumentContent = xmlDocument.OuterXml;
            var xDocumentContent = xDocument?.ToString(SaveOptions.DisableFormatting);

            Assert.AreEqual(xmlDocumentContent, xDocumentContent);
        }

        [TestMethod]
        public void ToXmlDocument()
        {
            const string xml =
                @"<!--root comment-->
                <root>
                    <child>content</child>
                </root>";

            var xDocument = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
            var xmlDocument = xDocument.ToXmlDocument();

            var xDocumentContent = xDocument.ToString(SaveOptions.DisableFormatting);
            var xmlDocumentContent = xmlDocument?.OuterXml;

            Assert.AreEqual(xDocumentContent, xmlDocumentContent);
        }

        [TestMethod]
        public void ToXmlElement()
        {
            var xElement = new XElement(@"Root", new XElement(@"Child", @"Content"));
            var xmlElement = xElement.ToXmlElement();

            var xElementContent = xElement.ToString(SaveOptions.DisableFormatting);
            var xmlElementContent = xmlElement.OuterXml;

            Assert.AreEqual(xElementContent, xmlElementContent);
        }

        [TestMethod]
        public void SetNamespace()
        {
            const string nsUri = @"http://dummy.com";
            XNamespace ns = nsUri;

            var xElement = new XElement(@"Node").SetNamespace(ns);

            Assert.AreEqual(ns, xElement.Name.Namespace);
        }
    }
}
