using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Models.submit
{
    public class CompSigRespModel
    {
        public string signature { get; set; }
        public string signerInfo { get; set; }
        public string description { get; set; }
        public string status { get; set; }
    }
}
