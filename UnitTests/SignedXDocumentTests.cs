using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ruttloff.SignedXDocument.UnitTests
{
    [TestClass]
    public class SignedXDocumentTests
    {
        [TestMethod]
        public void CloneConstructor()
        {
            const string xml =
                @"<!-- root comment -->
                <root>
                    <child>content</child>
                </root>";

            var xDocument = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
            var signedDocument = new SignedXDocument(xDocument);

            Assert.AreEqual(xDocument.ToString(), signedDocument.ToString());
        }

        [TestMethod]
        public void Signing()
        {
            const string xml =
                @"<!-- root comment -->
                <root>
                    <child>content</child>
                </root>";

            var xDocument = XDocument.Parse(xml);
            var signedDocument = new SignedXDocument(xDocument);

            Assert.AreNotEqual(xDocument.ToString(), signedDocument.Sign().ToString());
        }

        [TestMethod]
        public void SigningWithProperties()
        {
            const string xml =
                @"<!-- root comment -->
                <root>
                    <child>content</child>
                </root>";

            var xDocument = XDocument.Parse(xml);
            var signedDocument = new SignedXDocument(xDocument)
            {
                SignatureProperties = new List<XElement>
                {
                    new XElement(@"Property1", @"SomeValue1"),
                    new XElement(@"Property2", @"SomeValue2")
                }
            };

            Assert.AreNotEqual(xDocument.ToString(), signedDocument.Sign().ToString());
        }

        [TestMethod]
        public void Verification()
        {
            const string xml =
                @"<!-- root comment -->
                <root>
                    <child>content</child>
                </root>";

            var xDocument = XDocument.Parse(xml);
            var signedDocument = new SignedXDocument(xDocument);
            var withSignature = signedDocument.Sign();

            Assert.IsTrue(SignedXDocument.Verify(withSignature));
        }

        [TestMethod]
        public void VerificationAltered()
        {
            const string xml =
                @"<!-- root comment -->
                <root>
                    <child>content</child>
                </root>";

            var xDocument = XDocument.Parse(xml);
            var signedDocument = new SignedXDocument(xDocument);
            var withSignature = signedDocument.Sign();
            var altered = new XDocument(withSignature);
            altered.Root?.Add(new XElement(@"dummy"));

            Assert.IsFalse(SignedXDocument.Verify(altered));
        }

        [TestMethod]
        public void VerificationWithProperties()
        {
            const string xml =
                @"<!-- root comment -->
                <root>
                    <child>content</child>
                </root>";

            var xDocument = XDocument.Parse(xml);
            var signedDocument = new SignedXDocument(xDocument)
            {
                SignatureProperties = new List<XElement>
                {
                    new XElement(@"SomeProperty", @"SomeValue")
                }
            };
            var withSignature = signedDocument.Sign();

            Assert.IsTrue(SignedXDocument.Verify(withSignature));
        }

        [TestMethod]
        public void VerificationWithPropertiesAltered()
        {
            const string xml =
                @"<!-- root comment -->
                <root>
                    <child>content</child>
                </root>";

            var xDocument = XDocument.Parse(xml);
            var signedDocument = new SignedXDocument(xDocument)
            {
                SignatureProperties = new List<XElement>
                {
                    new XElement(@"SomeProperty", @"SomeValue", new [] {new XAttribute(@"Id", @"property")})
                }
            };
            var withSignature = signedDocument.Sign();
            var altered = new XDocument(withSignature);
            var propertyNode = altered.XPathSelectElement(@"//*[@Id='property']");
            propertyNode?.SetValue(@"SomeAlteredValue");

            Assert.IsFalse(SignedXDocument.Verify(altered));
        }
    }
}
