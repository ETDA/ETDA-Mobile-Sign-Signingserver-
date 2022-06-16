using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Models.signing_sign
{
    public class HashReqModel
    {
        [Required]
        public string request_id { get; set; }
        public string signerCert { get; set; }
        public string issuerCert { get; set; }
    }
}
