using SigningServer_TedaSign.Models.signing_sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text;

namespace SigningServer_TedaSign.Services
{
    public class SignInfoService
    {
        private readonly ILoggerFactory _loggerFactory;
        public SignInfoService(ILoggerFactory loggerFactory)
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
        //PDF
        public SignSignRespModel request_signedbytes(string callback_url, string digest, string signerCert, string issuerCert)
        {
            //var result = "{\"description\":\"Create SignInfo\",\"signedInfo\":\"SignInfo Context\",\"signatureId\":\"SignId Context\",\"status\":\"SUCCESS\"}";
            //SignSignRespModel signInfo_resp = JsonConvert.DeserializeObject<SignSignRespModel>(result);

            var _logger = _loggerFactory.CreateLogger<SignInfoService>();
            SignSignRespModel signInfo_resp;

            try
            {
                _logger.LogInformation("Curl to: " + callback_url);

                var signInfo_req_body = new SignInfoReqModel();
                signInfo_req_body.digest = digest;
                signInfo_req_body.signerCert = signerCert;
                signInfo_req_body.issuerCert = issuerCert;


                var client = new HttpClient();
                //client.DefaultRequestHeaders.Add("Authorization", "TOKEN");
                client.Timeout = TimeSpan.FromMinutes(1);
                var response = client.PostAsJsonAsync(callback_url, signInfo_req_body).Result;

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    _logger.LogInformation("Create SignInfo Response: OK");

                    var result = response.Content.ReadAsStringAsync().Result;
                    signInfo_resp = JsonConvert.DeserializeObject<SignSignRespModel>(result);

                    _logger.LogInformation("Create SignInfo Response Message: " + result);
                }
                else
                {
                    signInfo_resp = null;
                    _logger.LogError("Create SignInfo Response ERROR: HTTP Code " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                signInfo_resp = null;
                _logger.LogError("Curl to Create SignInfo ERROR: " + ex.Message);
                ex.StackTrace.ToString();
            }

            return signInfo_resp;
        }

        //XML
        public SignSignRespModel request_signInfo(string callback_url, string digest, string signerCert, string issuerCert, string xml_namespace, string signatureMethod, string digestMethod)
        {
            //var result = "{\"description\":\"\",\"signedInfo\":\"SignInfo Context\",\"signatureId\":\"SignId Context\",\"status\":\"SUCCESS\"}";
            //SignSignRespModel signInfo_resp = JsonConvert.DeserializeObject<SignSignRespModel>(result);


            var _logger = _loggerFactory.CreateLogger<SignInfoService>();
            SignSignRespModel signInfo_resp;

            try
            {
                _logger.LogInformation("Curl to: " + callback_url);

                var signInfo_req_body = new SignInfoReqModel();
                signInfo_req_body.digest = digest;
                signInfo_req_body.signerCert = signerCert;
                signInfo_req_body.issuerCert = issuerCert;
                signInfo_req_body.xml_namespace = xml_namespace;
                signInfo_req_body.digestMethod = digestMethod;
                signInfo_req_body.signatureMethod = signatureMethod;

                var req_str = JsonConvert.SerializeObject(signInfo_req_body);
                var req = req_str.Replace("xml_namespace", "namespace");

                var stringContent = new StringContent(req,  UnicodeEncoding.UTF8, "application/json");

                var client = new HttpClient();
                //client.DefaultRequestHeaders.Add("Authorization", "TOKEN");
                client.Timeout = TimeSpan.FromMinutes(1);
                var response = client.PostAsync(callback_url, stringContent).Result;

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    _logger.LogInformation("Create SignInfo Response: OK");

                    var result = response.Content.ReadAsStringAsync().Result;
                    signInfo_resp = JsonConvert.DeserializeObject<SignSignRespModel>(result);

                    _logger.LogInformation("Create SignInfo Response Message: " + result);

                }
                else
                {
                    _logger.LogError("Create SignInfo Response ERROR: HTTP Code " + response.StatusCode);

                    var result = response.Content.ReadAsStringAsync().Result;
                    signInfo_resp = JsonConvert.DeserializeObject<SignSignRespModel>(result);
                    
                    _logger.LogError("Create SignInfo Response Message: " + result);
                }
            }
            catch (Exception ex)
            {
                signInfo_resp = null;
                _logger.LogError("Curl to Create SignInfo Exception: " + ex.Message);
                ex.StackTrace.ToString();
            }

            return signInfo_resp;
        }
    }


}
