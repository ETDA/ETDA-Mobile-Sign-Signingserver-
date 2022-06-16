using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Models
{
    public class ConfigSettings : IConfigSettings
    {
        public string Signature_Method { get; set; }

    }
    public interface IConfigSettings
    {
        public string Signature_Method { get; set; }
    }
}
