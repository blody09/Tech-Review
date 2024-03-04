using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;


namespace StargateAPI.Business.Commands
{
    [ExcludeFromCodeCoverage]
    public class CreateLogEntry : IRequest<CreateLogEntryResult>
    {
        public int Id { get; set; }
        //TODO Time permitting i would add a class for logtype instead of a string. like an enum defined elsewhere for succcessfull entries. 
        public string LogType { get; set; }

        public string Message { get; set; }

        public string LogException { get; set; }

        public DateTime TimeStamp { get; set; }
    }

    
    public class CreateLogEntryHandler : IRequestHandler<CreateLogEntry, CreateLogEntryResult>
    {
        private readonly StargateContext _context;
      
        public CreateLogEntryHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<CreateLogEntryResult> Handle(CreateLogEntry request, CancellationToken cancellationToken)
        {

            var newLogEntry = new Logging()
            {
                LogType = request.LogType,
                Message = request.Message,
                LogException = request.LogException,
                TimeStamp = DateTime.Now
            };


            await _context.Logs.AddAsync(newLogEntry);
            await _context.SaveChangesAsync();

             return new CreateLogEntryResult()
             {
                  Id = newLogEntry.Id
             };


        }

    }

    public class CreateLogEntryResult : BaseResponse
    {
        public int Id { get; set; }
    }
}
