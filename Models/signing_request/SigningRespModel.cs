using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Models.signing_request
{
    public class SigningRespModel
    {
        public string result { get; set; }
        public string description { get; set; }
        public string request_id { get; set; }
        public string requesting_token { get; set; }
        public string signing_endpoint { get; set; }
        
    }
}
