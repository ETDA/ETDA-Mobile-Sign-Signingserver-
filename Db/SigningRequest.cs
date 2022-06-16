using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;


namespace SigningServer_TedaSign.Db
{
    public class SigningRequest
    {

        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userid")]
        public string userid { get; set; }

        [BsonElement("documentcategory")]
        [Required]
        public string documentcategory { get; set; }

        [BsonElement("refnumber")]
        [Required]
        public string refnumber { get; set; }


        [BsonElement("callback_url")]
        public CallbackURL callback_url { get; set; }


        [BsonElement("requesttoken")]
        public string requesttoken { get; set; }

        [BsonElement("hashresult")]
        public HashResult hashresult { get; set; }

        [BsonElement("signedinfo")]
        public SignedInfo signedinfo { get; set; }

        [BsonElement("clientid")]
        public string clientid { get; set; }


        [BsonElement("signerCert")]
        public string signerCert { get; set; }


        public SigningRequest()
        {

        }

        public SigningRequest(string userid, string documentcategory, string refnumber, string clientid, CallbackURL callback_url,
            string requesttoken)
        {
            this.userid = userid;
            this.documentcategory = documentcategory;
            this.refnumber = refnumber;
            this.clientid = clientid;
            this.callback_url = callback_url;
            this.requesttoken = requesttoken;
        }

        public SigningRequest(string Id, string userid, string documentcategory, string refnumber, string clientid, CallbackURL callback_url,
            string requesttoken, HashResult hashresult, string signerCert)
        {
            this.Id = Id;
            this.userid = userid;
            this.documentcategory = documentcategory;
            this.refnumber = refnumber;
            this.clientid = clientid;
            this.requesttoken = requesttoken;
            this.callback_url = callback_url;
            this.hashresult = hashresult;
            this.signerCert = signerCert;
        }

        public SigningRequest(string Id, string userid, string documentcategory, string refnumber, string clientid, CallbackURL callback_url,
            string requesttoken, HashResult hashresult, string signerCert, SignedInfo signedinfo)
        {
            this.Id = Id;
            this.userid = userid;
            this.documentcategory = documentcategory;
            this.refnumber = refnumber;
            this.clientid = clientid;
            this.requesttoken = requesttoken;
            this.callback_url = callback_url;
            this.hashresult = hashresult;
            this.signerCert = signerCert;
            this.signedinfo = signedinfo;
        }
    }

    public class CallbackURL
    {

        [BsonElement("hash")]
        public string hash { get; set; }

        [BsonElement("getsignature")]
        public string getsignature { get; set; }

        public CallbackURL(string hash, string getsignature)
        {
            this.hash = hash;
            this.getsignature = getsignature;
        }

    }

    public class HashResult
    {

        [BsonElement("result")]
        public string result { get; set; }

        [BsonElement("description")]
        public string description { get; set; }

        [BsonElement("documenthash")]
        public string documenthash { get; set; }

        [BsonElement("digestmethod")]
        public string digestmethod { get; set; }




        public HashResult(string result, string description, string documenthash, string digestmethod)
        {
            this.result = result;
            this.description = description;
            this.documenthash = documenthash;
            this.digestmethod = digestmethod;
        }

    }

    public class SignedInfo
    {

        [BsonElement("signatureid")]
        public string signatureid { get; set; }

        [BsonElement("signedinfo")]
        public string signedinfo { get; set; }

        [BsonElement("signedBytes")]
        public string signedBytes { get; set; }

        [BsonElement("xadessignedproperties")]
        public string xadessignedproperties { get; set; }

        [BsonElement("signedinfodigest")]
        public string signedinfodigest { get; set; }

        [BsonElement("description")]
        public string description { get; set; }

        [BsonElement("status")]
        public string status { get; set; }

        public SignedInfo(string signatureid, string signedinfo, string signedBytes, string xadessignedproperties, string signedinfodigest, string description, string status)
        {
            this.signatureid = signatureid;
            this.signedinfo = signedinfo;
            this.signedBytes = signedBytes;
            this.xadessignedproperties = xadessignedproperties;
            this.signedinfodigest = signedinfodigest;
            this.description = description;
            this.status = status;
        }

    }

}
