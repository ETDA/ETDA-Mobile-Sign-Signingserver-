using SigningServer_TedaSign.Models.signing_sign;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using SigningServer_TedaSign.Controllers;

namespace SigningServer_TedaSign.Services
{
    public class HashRequestService
    {
        private readonly ILoggerFactory _loggerFactory;
        public HashRequestService(ILoggerFactory loggerFactory)
        {
            loggerFactory =
               LoggerFactory.Create(builder =>
                   builder.AddSimpleConsole(options =>
                   {
                       options.IncludeScopes = true;
                       options.SingleLine = true;
                       options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss]: ";
                   }));
            this._loggerFactory = loggerFactory;
        }

        public HashRespModel request_hashAsync(string callback_url, string request_id, string signerCert, string issuerCert)
        {
            var _logger = _loggerFactory.CreateLogger<HashRequestService>();

            HashRespModel hash_resp;
            try
            {

                _logger.LogInformation("Curl to: " + callback_url);

                var hash_req_body = new HashReqModel();
                hash_req_body.request_id = request_id;
                hash_req_body.signerCert = signerCert;
                hash_req_body.issuerCert = issuerCert;

                var client = new HttpClient();
                //client.DefaultRequestHeaders.Add("Authorization", "TOKEN");
                client.Timeout = TimeSpan.FromMinutes(1);
                var response = client.PostAsJsonAsync(callback_url, hash_req_body).Result;

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    _logger.LogInformation("Doc Server Response: OK");

                    var result = response.Content.ReadAsStringAsync().Result;
                    hash_resp = JsonConvert.DeserializeObject<HashRespModel>(result);

                    _logger.LogInformation("Doc Server Response Message: " + result);
                }
                else
                {
                    hash_resp = null;
                    _logger.LogError("Doc Server Response ERROR: HTTP Status Code " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                hash_resp = null;
                _logger.LogError("Curl to Doc Server ERROR: " + ex.Message);
                ex.StackTrace.ToString();
            }

            return hash_resp;
        }
    }
}
