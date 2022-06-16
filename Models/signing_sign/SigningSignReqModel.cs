using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Models.signing_sign
{
    public class SigningSignReqModel
    {
        [Required]
        public Key key { get; set; }
    }

    public class Key
    {
        [Required]
        public string cert { get; set; }

        [Required]
        public string chains { get; set; }
    }

    public class SigningSign_Header
    {
        [FromHeader]
        [Required]
        public string Token { get; set; }
    }
}
