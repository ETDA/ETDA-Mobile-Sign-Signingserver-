using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Models.submit
{
    public class SubmitReqModel
    {
        [Required]
        public string signature { get; set; }
    }

    public class Submit_Header
    {
        [FromHeader]
        [Required]
        public string Token { get; set; }
    }
}
