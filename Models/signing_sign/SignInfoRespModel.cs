using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Models.signing_sign
{
    public class SignSignRespModel
    {

        public string signedInfo { get; set; }
        public string signedBytes { get; set; }
        public string signatureId { get; set; }
        public string xadesSignedProperties { get; set; }
        public string singedInfoDigest { get; set; }
        public string description { get; set; }
        public string status { get; set; }
    }
}
