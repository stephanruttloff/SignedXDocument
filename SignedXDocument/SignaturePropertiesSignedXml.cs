using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Ruttloff.SignedXDocument
{
    internal sealed class SignaturePropertiesSignedXml : SignedXml
    {
        #region Fields

        private readonly XmlDocument
            _document;

        private XmlElement
            _signaturePropertiesRoot;

        private readonly string
            _propertiesId;

        #endregion

        #region Constructor

        public SignaturePropertiesSignedXml(XmlDocument document, string signatureId, string propertiesId)
            : base(document)
        {
            if(string.IsNullOrEmpty(signatureId))
                throw new ArgumentException($"{nameof(signatureId)} can't be empty", nameof(signatureId));
            if (string.IsNullOrEmpty(propertiesId))
                throw new ArgumentException($"{nameof(propertiesId)} can't be empty", nameof(propertiesId));

            _document = document;
            Signature.Id = signatureId;
            _propertiesId = propertiesId;
        }

        #endregion

        #region Methods

        public void AddSignatureProperty(XmlElement property)
        {
            if(property == null)
                throw new ArgumentNullException(nameof(property));
            if(string.Equals(property.NamespaceURI, XmlDsigNamespaceUrl, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(@"Signature properties must not be in the XML Digital Signature namespace");

            AddSignatureProportiesRoot();

            var wrapper = _document.CreateElement(@"SignatureProperty", XmlDsigNamespaceUrl);
            wrapper.SetAttribute(@"Target", $"#{Signature.Id}");
            wrapper.AppendChild(_document.ImportNode(property, true));

            _signaturePropertiesRoot.AppendChild(wrapper);
        }

        public void AddSignatureProperties(IEnumerable<XmlElement> properties)
        {
            foreach (var property in properties)
                AddSignatureProperty(property);
        }

        private void AddSignatureProportiesRoot()
        {
            if (_signaturePropertiesRoot != null)
                return;

            _signaturePropertiesRoot = _document.CreateElement(@"SignatureProperties", XmlDsigNamespaceUrl);
            _signaturePropertiesRoot.SetAttribute(@"Id", _propertiesId);

            var signedProperties = new DataObject
            {
                Data = _signaturePropertiesRoot.SelectNodes(@".")
            };
            AddObject(signedProperties);

            var propertiesReference = new Reference($"#{_propertiesId}") { Type = @"http://www.w3.org/2000/02/xmldsig#SignatureProperty" };
            AddReference(propertiesReference);
        }

        #endregion

        #region Overrides

        public override XmlElement GetIdElement(XmlDocument document, string idValue)
        {
            if (string.IsNullOrEmpty(idValue))
                return null;

            if(string.Equals(idValue, _signaturePropertiesRoot?.GetAttribute(@"Id"), StringComparison.OrdinalIgnoreCase))
                return _signaturePropertiesRoot;

            return base.GetIdElement(document, idValue);
        }

        #endregion
    }
}
