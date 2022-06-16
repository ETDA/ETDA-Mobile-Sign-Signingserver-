using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SigningServer_TedaSign.Db;
using SigningServer_TedaSign.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Services
{

    public class ExternalClientService
    {
       
        private readonly IMongoCollection<ExternalClient> exClients;
        public ExternalClientService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            exClients = database.GetCollection<ExternalClient>(settings.ExternalClientCollectionName);
        }

        public ExternalClientService()
        {
        }

        public List<ExternalClient> Get()
        {
            return exClients.Find(exClient => true).ToList();
        }

        public ExternalClient FindByClientId(string clientid, string secret)
        {
            return exClients.Find(exClient => exClient.clientid == clientid && exClient.secret == secret).FirstOrDefault();
        }

        public ExternalClient Create(ExternalClient exClient)
        {
            exClients.InsertOne(exClient);
            return exClient;
        }

        public void Update(string id, ExternalClient exClientIn)
        {
            exClients.ReplaceOne(exClient => exClient.Id == id, exClientIn);
        }

        public void Remove(ExternalClient exClientIn)
        {
            exClients.DeleteOne(exClient => exClient.Id == exClientIn.Id);
        }

        public void Remove(string id)
        {
            exClients.DeleteOne(exClient => exClient.Id == id);
        }

    }
}
