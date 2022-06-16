using Microsoft.AspNetCore.Mvc;
using System;
using SigningServer_TedaSign.Models;
using SigningServer_TedaSign.Models.signing_sign;
using SigningServer_TedaSign.Services;
using SigningServer_TedaSign.Db;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace SigningServer_TedaSign.Controllers
{
    [ApiController]
    [Route("api/v2")]
    public class SigningSignController : Controller
    {
        private static readonly string ACCEPT = "accept";
        private static readonly string REJECT = "reject";

        private readonly ICallbackSettings _callbackurl;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfigSettings _config;

        private readonly SigningRequestService _signReqService;


        public SigningSignController(SigningRequestService signReqService, ICallbackSettings callbackurl, IConfigSettings config, ILoggerFactory loggerFactory)
        {
            this._callbackurl = callbackurl;
            this._signReqService = signReqService;
            this._config = config;

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

        [HttpPost("signing_sign/{request_id}")]
        [RequestSizeLimit(100_000_000)]
        public ActionResult signingSign([FromBody] SigningSignReqModel body, [FromHeader] SigningSign_Header header, string request_id)
        {

            var _logger = _loggerFactory.CreateLogger<SigningSignController>();

            SigningErrorModel errorResp = new SigningErrorModel();
            SigningSignRespModel signing_sign_resp = new SigningSignRespModel();

            if (ModelState.IsValid)
            {

                try
                {
                    _logger.LogInformation("********** Start Signing Sign Session() **********");

                    //check Authorized
                    var signReq_query = _signReqService.FindByRequesttoken(header.Token);
                    if (signReq_query == null)
                    {
                        errorResp.result = REJECT;
                        errorResp.description = "Token is Invalid";
                        _logger.LogError("Token is Invalid");

                        return BadRequest(errorResp);
                    }
                    else
                    {

                        if (!request_id.Equals(signReq_query.Id))
                        {
                            errorResp.result = REJECT;
                            errorResp.description = "Request Id is Invalid";
                            _logger.LogError("Request Id is Invalid");

                            return BadRequest(errorResp);
                        }

                        _logger.LogInformation("Request_id: " + signReq_query.Id);
                        _logger.LogInformation("Request Token: " + header.Token);

                        //request documant hash

                        var hash_resp = requestHash(signReq_query.callback_url.hash, signReq_query.Id, body.key.cert, body.key.chains, _logger);
                        if (hash_resp == null)
                        {
                            errorResp.result = REJECT;
                            errorResp.description = "Curl to document hash Exception";
                            return StatusCode(StatusCodes.Status500InternalServerError, errorResp);
                        }
                        else
                        {
                            if (hash_resp.result.Equals("accept"))
                            {
                                if (String.IsNullOrEmpty(hash_resp.document_hash))
                                {
                                    errorResp.result = REJECT;
                                    errorResp.description = "Document hash is Invalid";
                                    _logger.LogError("Document hash is Invalid");
                                    return Ok(errorResp);
                                }
                                else
                                {
                                    var hash_result = new HashResult(
                                        hash_resp.result,
                                        hash_resp.description,
                                        hash_resp.document_hash,
                                        hash_resp.digest_method);

                                    var update_hash_db = new SigningRequest(
                                        signReq_query.Id,
                                        signReq_query.userid,
                                        signReq_query.documentcategory,
                                        signReq_query.refnumber,
                                        signReq_query.clientid,
                                        signReq_query.callback_url,
                                        signReq_query.requesttoken,
                                        hash_result,
                                        body.key.cert);

                                    _signReqService.Update(signReq_query.Id, update_hash_db);

                                    _logger.LogInformation("Update Hash Result to DB SUCCESS");

                                    //creat Sign Info
                                    SignSignRespModel signInfo_resp = new SignSignRespModel();

                                    if (signReq_query.documentcategory.Equals("PDF"))
                                    {
                                        signInfo_resp = createSignedBytesPDF(
                                            _callbackurl.CreateSignedBytes_Url_PDF,
                                            hash_resp.document_hash,
                                            body.key.cert,
                                            body.key.chains,
                                            _logger);
                                    }
                                    else if (signReq_query.documentcategory.Equals("XML"))
                                    {
                                        signInfo_resp = creatSignInfoXML(
                                            _callbackurl.CreateSignInfo_Url_XML,
                                            hash_resp.document_hash,
                                            body.key.cert,
                                            body.key.chains,
                                            hash_resp.xml_namespace,
                                            _config.Signature_Method,
                                            hash_resp.digest_method,
                                            _logger);
                                    }

                                    if (signInfo_resp == null)
                                    {
                                        errorResp.result = REJECT;
                                        errorResp.description = "Curl to Create SignInfo Exception";
                                        return StatusCode(StatusCodes.Status500InternalServerError, errorResp);
                                    }
                                    else
                                    {

                                        if (signInfo_resp.status.Equals("SUCCESS"))
                                        {
                                            var sign_info_result = new SignedInfo(
                                                signInfo_resp.signatureId,
                                                signInfo_resp.signedInfo,
                                                signInfo_resp.signedBytes,
                                                signInfo_resp.xadesSignedProperties,
                                                signInfo_resp.singedInfoDigest,
                                                signInfo_resp.description,
                                                signInfo_resp.status);

                                            var update_signInfo_db = new SigningRequest(
                                                signReq_query.Id,
                                                signReq_query.userid,
                                                signReq_query.documentcategory,
                                                signReq_query.refnumber,
                                                signReq_query.clientid,
                                                signReq_query.callback_url,
                                                signReq_query.requesttoken,
                                                hash_result,
                                                body.key.cert,
                                                sign_info_result);

                                            _signReqService.Update(signReq_query.Id, update_signInfo_db);

                                            _logger.LogInformation("Update SignInfo Result to DB SUCCESS");

                                            signing_sign_resp = new SigningSignRespModel();
                                            signing_sign_resp.result = ACCEPT;
                                            signing_sign_resp.description = "Create sign info success";
                                            signing_sign_resp.request_token = update_hash_db.requesttoken;

                                            if (signReq_query.documentcategory.Equals("PDF"))
                                            {
                                                signing_sign_resp.signedInfo = signInfo_resp.signedBytes;
                                            }
                                            else
                                            {
                                                signing_sign_resp.signedInfo = signInfo_resp.signedInfo;
                                            }

                                            _logger.LogInformation("Create Sign Info SUCCESS");
                                        }
                                        else
                                        {
                                            errorResp.result = REJECT;
                                            errorResp.description = signInfo_resp.description;
                                            _logger.LogError(signInfo_resp.description);
                                            return BadRequest(errorResp);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                errorResp.result = hash_resp.result;
                                errorResp.description = hash_resp.description;
                                _logger.LogError(hash_resp.description);
                                return Ok(errorResp);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorResp.result = REJECT;
                    errorResp.description = ex.Message;
                    _logger.LogError("Unknown ERROR: " + ex.Message);
                    ex.StackTrace.ToString();
                    return BadRequest(errorResp);
                }
                finally
                {
                    _logger.LogInformation("********** End Signing Sign Session() **********");
                }

            }
            return Ok(signing_sign_resp);
        }

        private HashRespModel requestHash(string callback_url, string request_id, string signerCert, string issuerCert, ILogger<SigningSignController> _logger)
        {
            try
            {
                _logger.LogInformation("----- Start Request Document Hash form Doc Server -----");
                HashRequestService hash_req = new HashRequestService(_loggerFactory);
                return hash_req.request_hashAsync(callback_url, request_id, signerCert, issuerCert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ex.StackTrace.ToString();
                return null;
            }
        }

        private SignSignRespModel createSignedBytesPDF(string callback_url, string digest, string signerCert, string issuerCert, ILogger<SigningSignController> _logger)
        {
            try
            {
                _logger.LogInformation("----- Start Request Creat SignInfo PDF -----");
                SignInfoService sign_info_req = new SignInfoService(_loggerFactory);
                return sign_info_req.request_signedbytes(callback_url, digest, signerCert, issuerCert);
            }
            catch (Exception ex)
            {
                ex.StackTrace.ToString();
                return null;
            }
        }

        private SignSignRespModel creatSignInfoXML(string callback_url, string digest, string signerCert,
            string issuerCert, string xml_namespace, string signatureMethod, string digestMethod, ILogger<SigningSignController> _logger)
        {
            try
            {
                _logger.LogInformation("----- Start Request to Creat SignInfo XML -----");
                SignInfoService sign_info_req = new SignInfoService(_loggerFactory);
                return sign_info_req.request_signInfo(callback_url, digest, signerCert, issuerCert, xml_namespace, signatureMethod, digestMethod);
            }
            catch (Exception ex)
            {
                ex.StackTrace.ToString();
                return null;
            }
        }

    }
}
