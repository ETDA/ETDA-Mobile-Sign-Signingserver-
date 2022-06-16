using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Models
{
    public class SigningReqModel
    {
        public string user_id { get; set; }

        [Required]
        public string document_category { get; set; }

        [Required]
        public string ref_number { get; set; }

        [Required]
        public Callback callback_url { get; set; }

    }

    public class Header
    {
        [FromHeader]
        [Required]
        public string ClientID { get; set; }
        [FromHeader]
        [Required]
        public string Secret { get; set; }
    }

    public class Callback
    {
        [Required]
        public string hash { get; set; }
        [Required]
        public string getsignature { get; set; }
    }
}
