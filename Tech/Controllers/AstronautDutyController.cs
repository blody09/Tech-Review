using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AstronautDutyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly StargateContext _context;
        public AstronautDutyController(IMediator mediator, StargateContext context)
        {
            _mediator = mediator;
            _context = context;
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

                var logging = new Logging()
                {
                    LogType = "Successs",
                    Message = $"Successfully retrieved Astronaut Duties by Name ",
                    LogException = string.Empty,
                    TimeStamp = DateTime.Now,
                };

                _context.Logs.Add(logging);
                await _context.SaveChangesAsync();

   
                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                var logging = new Logging()
                {
                    LogType = "Bad Request",
                    Message = $"Failed to retrieved Astronaut Duties by Name ",
                    LogException = string.Empty,
                    TimeStamp = DateTime.Now,
                };

                _context.Logs.Add(logging);
                await _context.SaveChangesAsync();

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