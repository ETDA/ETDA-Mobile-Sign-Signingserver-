using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Db
{
    public class ExternalClient
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("clientid")]
        [Required]
        public string clientid { get; set; }

        [BsonElement("domain")]
        [Required]
        public string domain { get; set; }

        [BsonElement("name")]
        [Required]
        public string name { get; set; }

        [BsonElement("secret")]
        [Required]
        public string secret { get; set; }
    }
}
