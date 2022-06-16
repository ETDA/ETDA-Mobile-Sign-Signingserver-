using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SigningServer_TedaSign.Models;
using SigningServer_TedaSign.Models.get_signature;
using SigningServer_TedaSign.Models.submit;
using SigningServer_TedaSign.Services;
using System;

namespace SigningServer_TedaSign.Controllers
{
    [ApiController]
    [Route("api/v2")]
    public class SubmitController : Controller
    {
        private static readonly string ACCEPT = "accept";
        private static readonly string REJECT = "reject";

        private readonly ICallbackSettings _callbackurl;
        private readonly ILoggerFactory _loggerFactory;


        private readonly SigningRequestService _signReqService;

        public SubmitController(SigningRequestService signReqService, ICallbackSettings callbackurl, IConfigSettings config, ILoggerFactory loggerFactory)
        {
            this._callbackurl = callbackurl;
            this._signReqService = signReqService;


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


        [HttpPost("signing_sign/{request_id}/submit")]
        [RequestSizeLimit(100_000_000)]
        public ActionResult submit([FromBody] SubmitReqModel body, [FromHeader] Submit_Header header, string request_id)
        {
            var _logger = _loggerFactory.CreateLogger<SubmitController>();
            SigningErrorModel errorResp = new SigningErrorModel();
            SubmitRespModel submit_resp = new SubmitRespModel();

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("********** Start Signing Request Session() **********");
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
                        else
                        {

                            _logger.LogInformation("Request_id: " + signReq_query.Id);
                            _logger.LogInformation("Request Token: " + header.Token);

                            CompSigRespModel compsig_resp = new CompSigRespModel();

                            if (signReq_query.documentcategory.Equals("PDF"))
                            {
                                compsig_resp = createSignaturePDF(_callbackurl.ComposeSignerInfo_Url_PDF,
                                    signReq_query.signedinfo.signatureid,
                                    body.signature,
                                    signReq_query.signedinfo.signedBytes,
                                    _logger);
                            }
                            else if (signReq_query.documentcategory.Equals("XML"))
                            {
                                compsig_resp = createSignatureXML(_callbackurl.ComposeSignature_Url_XML,
                                    signReq_query.signedinfo.signatureid,
                                    body.signature,
                                    signReq_query.signedinfo.signedinfo,
                                    signReq_query.signedinfo.xadessignedproperties,
                                    signReq_query.signerCert,
                                    _logger);
                            }

                            if (compsig_resp == null)
                            {
                                errorResp.result = REJECT;
                                errorResp.description = "Compose Signature Exception";

                                return StatusCode(StatusCodes.Status500InternalServerError, errorResp);
                            }
                            else
                            {

                                if (compsig_resp.status.Equals("SUCCESS"))
                                {

                                    if (signReq_query.documentcategory.Equals("PDF"))
                                    {
                                        compsig_resp.signature = compsig_resp.signerInfo;
                                    }

                                    var getsig_resp = get_signature(
                                        signReq_query.callback_url.getsignature,
                                        signReq_query.Id,
                                        signReq_query.userid,
                                        signReq_query.hashresult.documenthash,
                                        compsig_resp.signature,
                                        _logger);

                                    if (getsig_resp == null)
                                    {
                                        errorResp.result = REJECT;
                                        errorResp.description = "Can not reach to get signature value";
                                        _logger.LogError("Can not reach to get signature value");
                                        return Ok(errorResp);
                                    }
                                    else
                                    {
                                        if (getsig_resp.result.Equals("accept"))
                                        {
                                            submit_resp.result = ACCEPT;
                                            submit_resp.description = "signed document success";
                                            _logger.LogInformation("signed document success");

                                        }
                                        else
                                        {
                                            errorResp.result = getsig_resp.result;
                                            errorResp.description = getsig_resp.description;
                                            _logger.LogError(getsig_resp.description);
                                            return Ok(errorResp);
                                        }
                                    }
                                }
                                else
                                {
                                    errorResp.result = REJECT;
                                    errorResp.description = compsig_resp.description;
                                    _logger.LogError(compsig_resp.description);
                                    return BadRequest(errorResp);
                                }
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
                    _logger.LogInformation("********** End Signing Request Session() **********");
                }
            }

            return Ok(submit_resp);
        }

        private CompSigRespModel createSignaturePDF(string callback_url, string signatureId, string signatureValue, string signedBytes, ILogger<SubmitController> _logger)
        {
            try
            {
                _logger.LogInformation("----- Start Request Compose Signature PDF -----");
                ComposeSignatureService comp_req = new ComposeSignatureService(_loggerFactory);
                return comp_req.request_compsignedinfo(callback_url, signatureId, signatureValue, signedBytes);
            }
            catch (Exception ex)
            {
                ex.StackTrace.ToString();
                return null;
            }
        }

        private CompSigRespModel createSignatureXML(string callback_url, string signatureId, string signatureValue,
            string signedInfo, string xadesSignedProperties, string signerCert, ILogger<SubmitController> _logger)
        {
            try
            {
                _logger.LogInformation("----- Start Request Compose Signature XML -----");
                ComposeSignatureService comp_req = new ComposeSignatureService(_loggerFactory);
                return comp_req.request_compSignature(callback_url, signatureId, signatureValue, signedInfo, xadesSignedProperties, signerCert);
            }
            catch (Exception ex)
            {
                ex.StackTrace.ToString();
                return null;
            }
        }

        private GetSignatureRespModel get_signature(string callback_url, string request_id, string user_id, string document_hash, string signature, ILogger<SubmitController> _logger)
        {
            try
            {
                _logger.LogInformation("----- Start Request Get_signature -----");
                GetSignatureService hash_req = new GetSignatureService(_loggerFactory);
                return hash_req.get_signature(callback_url, request_id, user_id, document_hash, signature);
            }
            catch (Exception ex)
            {
                ex.StackTrace.ToString();
                return null;
            }
        }
    }
}
