using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateAPI.Controllers
{
   
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PersonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetPeople()
        {
            try
            {
                var result = await _mediator.Send(new GetPeople()
                {

                });
                var logEntry = new CreateLogEntry // would create a utility for this instead of having it all over. 
                {
                    LogType = "Success",
                    Message = $"Successfully Retrieved People"
                };
                var logHandler = new CreateLogEntry();
                var logResult = logHandler;

                await _mediator.Send(logResult);
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                var logEntry = new CreateLogEntry
                {
                    LogType = "Failure",
                    Message = $"Failed to Retrieve People"

                };
                var logHandler = new CreateLogEntry();
                var result = logHandler;

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetPersonByName(string name)
        {
            try
            {
                var result = await _mediator.Send(new GetPersonByName()
                {
                    Name = name
                });

                var logEntry = new CreateLogEntry
                {
                    LogType = "Success",
                    Message = $"Successfully Retrieved Person by their Name"
                };
                var logHandler = new CreateLogEntry();
                var logResult = logHandler;

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                var logEntry = new CreateLogEntry
                {
                    LogType = "Failure",
                    Message = $"Failed to Retrieve Person by their Name"

                };
                var logHandler = new CreateLogEntry();
                var result = logHandler;

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> CreatePerson([FromBody] string name)
        {
            try
            {
                var result = await _mediator.Send(new CreatePerson()
                {
                    Name = name
                });

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
    }
}