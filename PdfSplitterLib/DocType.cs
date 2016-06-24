using Newtonsoft.Json;

/// <summary>
/// Just a placeholder class used to receive the deserialization
/// of a json string.
/// </summary>
namespace PdfSplitterLib
{
    class DocType
    {
        //Document Type
        [JsonProperty(PropertyName = "document-type")]
        public string documentType;
    }
}
