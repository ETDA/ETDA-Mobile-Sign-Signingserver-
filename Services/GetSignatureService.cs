using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SigningServer_TedaSign.Models.get_signature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Services
{
    public class GetSignatureService
    {
        private readonly ILoggerFactory _loggerFactory;
        public GetSignatureService(ILoggerFactory loggerFactory)
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

        public GetSignatureRespModel get_signature(string callback_url, string request_id, string user_id, string document_hash, string signature)
        {

            var _logger = _loggerFactory.CreateLogger<GetSignatureService>();
            GetSignatureRespModel getsig_resp;
            try
            {
                _logger.LogInformation("Curl to: " + callback_url);

                var getsig_req_body = new GetSignatureReqModel();
                getsig_req_body.request_id = request_id;
                getsig_req_body.user_id = user_id;
                getsig_req_body.document_hash = document_hash;
                getsig_req_body.signature = signature;               

                var client = new HttpClient();
                //client.DefaultRequestHeaders.Add("Authorization", "TOKEN");
                client.Timeout = TimeSpan.FromMinutes(1);
                var response = client.PostAsJsonAsync(callback_url, getsig_req_body).Result;

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    _logger.LogInformation("Doc Server Response: OK");

                    var result = response.Content.ReadAsStringAsync().Result;
                    getsig_resp = JsonConvert.DeserializeObject<GetSignatureRespModel>(result);

                    _logger.LogInformation("Doc Server Response Message: " + result);
                }
                else
                {
                    getsig_resp = null;
                    _logger.LogError("Doc Server Response ERROR: HTTP Code " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                getsig_resp = null;
                _logger.LogError("Doc Server ERROR: " + ex.Message);
                ex.StackTrace.ToString();
            }

            return getsig_resp;
        }
    }
}
