using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Models.submit
{
    public class CompSigReqModel
    {
        public string signatureId { get; set; }
        public string signatureValue { get; set; }
        public string signedBytes { get; set; }
        public string signedInfo { get; set; }
        public string xadesSignedProperties { get; set; }
        public string signerCert { get; set; }
    }
}
