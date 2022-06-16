using Microsoft.AspNetCore.Mvc;
using System;
using SigningServer_TedaSign.Models.signing_request;
using SigningServer_TedaSign.Models;
using SigningServer_TedaSign.Services;
using SigningServer_TedaSign.Db;
using Microsoft.Extensions.Logging;

namespace SigningServer_TedaSign.Controllers
{

    [ApiController]
    [Route("api/v2")]
    public class SigningRequestController : Controller
    {
        private static readonly string ACCEPT = "accept";
        private static readonly string REJECT = "reject";

        private readonly ICallbackSettings _callbackurl;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ExternalClientService _extClientService;
        private readonly SigningRequestService _signReqService;



        public SigningRequestController(ExternalClientService extClientService, SigningRequestService signReqService, ICallbackSettings callbackurl, ILoggerFactory loggerFactory)
        {

            this._extClientService = extClientService;
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
            Console.WriteLine(extClientService);

        }


        [HttpPost("signing_sign/request")]
        [RequestSizeLimit(100_000_000)]
        public ActionResult signReg([FromBody] SigningReqModel body, [FromHeader] Header header)
        {

            var _logger = _loggerFactory.CreateLogger<SigningRequestController>();

            SigningRespModel signingResp = new SigningRespModel();
            SigningErrorModel errorResp = new SigningErrorModel();
            RequestToken reqToken = new RequestToken();

            if (ModelState.IsValid)
            {

                try
                {
                    _logger.LogInformation("********** Start Signing Request Session() **********");

                    //Authorized ClientID and SecretID
                    var authen = _extClientService.FindByClientId(header.ClientID, header.Secret);
                    if (authen == null)
                    {
                        errorResp.result = REJECT;
                        errorResp.description = "clientID or secretID Invalid";
                        _logger.LogError("clientID or secretID Invalid");
                        _logger.LogError("clientID: " + header.ClientID + ", " + "secretID: " + header.Secret);

                        return Unauthorized(errorResp);
                    }
                    else
                    {
                        if (body.document_category.Equals("PDF") || body.document_category.Equals("XML"))
                        {
                            _logger.LogInformation("user_id: " + body.user_id);
                            _logger.LogInformation("ref_number: " + body.ref_number);
                            _logger.LogInformation("file type: " + body.document_category);
                            _logger.LogInformation("callback url /hash: " + body.callback_url.hash);
                            _logger.LogInformation("callback url /getsignature: " + body.callback_url.getsignature);

                            var callback = new CallbackURL(body.callback_url.hash, body.callback_url.getsignature);

                            var requestToken = reqToken.generate(20);

                            _logger.LogInformation("Created Request_token: " + requestToken);

                            var signReqDB = new SigningRequest(
                                body.user_id,
                                body.document_category,
                                body.ref_number,
                                header.ClientID,
                                callback,
                                requestToken);

                            _signReqService.Create(signReqDB);

                            _logger.LogInformation("Updated Signing Request to DB SUCCESS");

                            signReqDB = _signReqService.FindByRequesttoken(signReqDB.requesttoken);

                            _logger.LogInformation("Created Request_id: " + signReqDB.Id);

                            signingResp.result = ACCEPT;
                            signingResp.description = "signing request success";
                            signingResp.request_id = signReqDB.Id;
                            signingResp.requesting_token = signReqDB.requesttoken;
                            signingResp.signing_endpoint = _callbackurl.Signing_Endpoint;

                            _logger.LogInformation("Signing Request SUCCESS");
                        }
                        else
                        {
                            errorResp.result = REJECT;
                            errorResp.description = "Invalid file type";
                            _logger.LogError("Invalid file type: " + body.document_category);
                            return Ok(errorResp);
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
            return Ok(signingResp);
        }
    }
}
