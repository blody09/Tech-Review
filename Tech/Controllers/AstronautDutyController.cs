using MediatR;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AstronautDutyController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AstronautDutyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetAstronautDutiesByName(string name)
        {
            try
            {
                var result = await _mediator.Send(new GetPersonByName()
                {
                    Name = name
                });

                //var logEntry = new CreateLogEntry
                //{
                //    LogType = "Success",
                //    Message = $"Successfully retrieved Astronaut Duties by Name"
                //};
                //var logHandler = new CreateLogEntry();
                //var logResult = logHandler;
                
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                //var logEntry = new CreateLogEntry
                //{
                //    LogType = "Failure",
                //    Message = $"Failed to Retrieve Astronaut Duties by Name"

                //};
                //var logHandler = new CreateLogEntry();
                //var result = logHandler;
                
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }            
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateAstronautDuty([FromBody] CreateAstronautDuty request)
        {
                var result = await _mediator.Send(request);
                return this.GetResponse(result);           
        }
    }
}