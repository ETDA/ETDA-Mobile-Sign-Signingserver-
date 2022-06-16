using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace SigningServer_TedaSign.Models.signing_sign
{
    public class SignInfoReqModel
    {
        [Required]
        public string digest { get; set; }
        [Required]
        public string signerCert { get; set; }
        [Required]
        public string issuerCert { get; set; }
        public string xml_namespace { get; set; }
        public string signatureMethod { get; set; }
        public string digestMethod { get; set; }
    }
}
