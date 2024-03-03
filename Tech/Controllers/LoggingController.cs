using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;


namespace StargateAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoggingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LoggingController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("{Logs}")]
        public async Task<IActionResult> GetLogs()
        {
            try
            {
                var result = await _mediator.Send(new GetLogs(){ });

                return this.GetResponse(result);
            }
            catch (Exception ex)
            { 
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost("")]
         public async Task<IActionResult> CreateLogEntry([FromBody] CreateLogEntry request)
        {
            try
            {
                var result = await _mediator.Send(request);
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                throw;
            };

        }



    }
}
