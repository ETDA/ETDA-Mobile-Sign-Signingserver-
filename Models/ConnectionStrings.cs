using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Models
{
    public class ConnectionStrings : IDatabaseSettings
    {
        public string ExternalClientCollectionName { get; set; }
        public string SigningRequestCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IDatabaseSettings
    {
        string ExternalClientCollectionName { get; set; }
        public string SigningRequestCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
