using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Models.signing_sign
{
    public class HashRespModel
    {
        public string result { get; set; }
        public string description { get; set; }
        public string document_hash { get; set; }
        public string xml_namespace { get; set; }
        public string digest_method { get; set; }
    }
}
