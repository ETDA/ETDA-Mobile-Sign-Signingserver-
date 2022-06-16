using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Models
{
    public class CallbackSettings : ICallbackSettings
    {
        public string Signing_Endpoint { get; set; }
        public string CreateSignedBytes_Url_PDF { get; set; }
        public string CreateSignInfo_Url_XML { get; set; }
        public string ComposeSignerInfo_Url_PDF { get; set; }
        public string ComposeSignature_Url_XML { get; set; }
    }

    public interface ICallbackSettings
    {
        public string Signing_Endpoint { get; set; }
        public string CreateSignedBytes_Url_PDF { get; set; }
        public string CreateSignInfo_Url_XML { get; set; }
        public string ComposeSignerInfo_Url_PDF { get; set; }
        public string ComposeSignature_Url_XML { get; set; }
    }
}
