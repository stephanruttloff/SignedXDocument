# SignedXDocument
Wrapper class to handle enveloped xmldsig in C#.

# Usage example

```c#
public XDocument SignMyDocument(XDocument document)
{
  var signedDocument = new SignedXDocument(document)
  {
      SignatureProperties = new List<XElement>
      {
          new XElement(@"Timestamp", DateTimeOffset.UtcNow)
      }
  };
  
  return signedDocument.Sign();
}

public bool VerifyMyDocument(XDocument signedDocument)
{
  return SignedXDocument.Verify(signedDocument);
}
```
