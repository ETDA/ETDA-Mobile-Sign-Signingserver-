using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Models.signing_sign
{
    public class SigningSignRespModel
    {
        public string result { get; set; }
        public string description { get; set; }
        public string request_token { get; set; }
        public string signedInfo { get; set; }

        
    }
}
