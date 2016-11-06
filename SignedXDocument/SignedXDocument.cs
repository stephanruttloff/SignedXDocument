using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Linq;

namespace Ruttloff.SignedXDocument
{
    public class SignedXDocument : XDocument
    {
        #region Fields

        private static readonly CspParameters
            _keyParameters = new CspParameters {KeyContainerName = "XML_DSIG_RSA_KEY"};

        private const string
            _signatureId = @"SignatureId",
            _propertiesId = @"PropertiesId";

        private static readonly XNamespace
            _propertyNs = @"http://ruttloff.org/SignedXDocument#property";

        #endregion

        #region Properties

        public IEnumerable<XElement> SignatureProperties { get; set; }

        #endregion

        #region Constructor

        public SignedXDocument(XDocument document) : base(document) {}

        #endregion

        #region Methods

        public XDocument Sign()
        {
            using (var key = new RSACryptoServiceProvider(_keyParameters))
            {
                var document = this.ToXmlDocument();
                var signer = new SignaturePropertiesSignedXml(document, _signatureId, _propertiesId) { SigningKey = key };

                var reference = new Reference(string.Empty);
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                signer.AddReference(reference);

                if (SignatureProperties?.Any() == true)
                    signer.AddSignatureProperties(SignatureProperties.Select(x => x.SetNamespace(_propertyNs).ToXmlElement()));

                signer.ComputeSignature();

                var signature = document.ImportNode(signer.GetXml(), true);
                document.DocumentElement?.AppendChild(signature);

                return document.ToXDocument();
            }
        }

        public static bool Verify(XDocument document)
        {
            var xmlDocument = document.ToXmlDocument();
            var signer = new SignaturePropertiesSignedXml(xmlDocument, _signatureId, _propertiesId);
            var nodeList = xmlDocument.GetElementsByTagName(@"Signature");

            signer.LoadXml((XmlElement) nodeList[0]);

            using (var key = new RSACryptoServiceProvider(_keyParameters))
                return signer.CheckSignature(key);
        }

        #endregion
    }
}
