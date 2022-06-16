using MongoDB.Driver;
using SigningServer_TedaSign.Db;
using SigningServer_TedaSign.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Services
{
    public class SigningRequestService
    {
        private readonly IMongoCollection<SigningRequest> signRequests;

        public SigningRequestService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            signRequests = database.GetCollection<SigningRequest>(settings.SigningRequestCollectionName);
        }

        public SigningRequestService()
        {
        }

        public List<SigningRequest> Get()
        {
            return signRequests.Find(exClient => true).ToList();
        }

        public SigningRequest FindByRequesttoken(string reqToken)
        {
            return signRequests.Find(signRequest => signRequest.requesttoken == reqToken).FirstOrDefault();
        }

        public void Create(SigningRequest signReqIn)
        {
            signRequests.InsertOne(signReqIn);
            //return signReqIn;
        }

        public void Update(string id, SigningRequest signReqIn)
        {
            signRequests.ReplaceOne(signRequest => signRequest.Id == id, signReqIn);
        }

        public void Remove(SigningRequest signReqIn)
        {
            signRequests.DeleteOne(signRequest => signRequest.Id == signReqIn.Id);
        }

        public void Remove(string id)
        {
            signRequests.DeleteOne(signRequest => signRequest.Id == id);
        }
    }
}
