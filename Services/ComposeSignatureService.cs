using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SigningServer_TedaSign.Models.submit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SigningServer_TedaSign.Services
{
    public class ComposeSignatureService
    {
        private readonly ILoggerFactory _loggerFactory;
        public ComposeSignatureService(ILoggerFactory loggerFactory)
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

        public CompSigRespModel request_compsignedinfo(string callback_url, string signatureId, string signatureValue, string signedBytes)
        {

            var _logger = _loggerFactory.CreateLogger<ComposeSignatureService>();

            CompSigRespModel compsig_resp;
            try
            {

                _logger.LogInformation("Curl to: " + callback_url);

                var compsig_req_body = new CompSigReqModel();
                compsig_req_body.signatureId = signatureId;
                compsig_req_body.signatureValue = signatureValue;
                compsig_req_body.signedBytes = signedBytes;

                var client = new HttpClient();
                //client.DefaultRequestHeaders.Add("Authorization", "TOKEN");
                client.Timeout = TimeSpan.FromMinutes(1);
                var response = client.PostAsJsonAsync(callback_url, compsig_req_body).Result;

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    _logger.LogInformation("Compose Signature Response: OK");

                    var result = response.Content.ReadAsStringAsync().Result;
                    compsig_resp = JsonConvert.DeserializeObject<CompSigRespModel>(result);

                    _logger.LogInformation("Compose Signature Response Message: " + result);
                }
                else
                {
                    compsig_resp = null;
                    _logger.LogError("Compose Signature Response ERROR: HTTP Code " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                compsig_resp = null;
                _logger.LogError("Compose Signature ERROR: " + ex.Message);
                ex.StackTrace.ToString();
            }

            return compsig_resp;
        }

        //XML
        public CompSigRespModel request_compSignature(string callback_url, string signatureId, string signatureValue, string signedInfo, string xadesSignedProperties, string signerCert)
        {

            var _logger = _loggerFactory.CreateLogger<ComposeSignatureService>();
            CompSigRespModel compsig_resp;
            try
            {
                _logger.LogInformation("Curl to: " + callback_url);
                var compsig_req_body = new CompSigReqModel();
                compsig_req_body.signatureId = signatureId;
                compsig_req_body.signatureValue = signatureValue;
                compsig_req_body.signedInfo = signedInfo;
                compsig_req_body.xadesSignedProperties = xadesSignedProperties;
                compsig_req_body.signerCert = signerCert;

                var client = new HttpClient();
                //client.DefaultRequestHeaders.Add("Authorization", "TOKEN");
                client.Timeout = TimeSpan.FromMinutes(1);
                var response = client.PostAsJsonAsync(callback_url, compsig_req_body).Result;

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    _logger.LogInformation("Compose Signature Response: OK");
                    var result = response.Content.ReadAsStringAsync().Result;
                    compsig_resp = JsonConvert.DeserializeObject<CompSigRespModel>(result);

                    _logger.LogInformation("Compose Signature Response Message: " + result);
                }
                else
                {
                    _logger.LogError("Compose Signature Response ERROR: HTTP Code " + response.StatusCode);

                    var result = response.Content.ReadAsStringAsync().Result;
                    compsig_resp = JsonConvert.DeserializeObject<CompSigRespModel>(result);

                    _logger.LogError("Compose Signature Response Message: " + result);
                }
            }
            catch (Exception ex)
            {
                compsig_resp = null;
                _logger.LogError("Compose Signature Exception: " + ex.Message);
                ex.StackTrace.ToString();
            }

            return compsig_resp;
        }
    }
}
