using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Models.get_signature
{
    public class GetSignatureReqModel
    {
        public string request_id { get; set; }

        public string user_id { get; set; }
        public string document_hash { get; set; }
        public string signature { get; set; }
    }
}
