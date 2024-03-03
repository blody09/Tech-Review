//mediatr? -- pattern
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class CreatePerson : IRequest<CreatePersonResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class CreatePersonPreProcessor : IRequestPreProcessor<CreatePerson>
    {
        private readonly StargateContext _context;
        public CreatePersonPreProcessor(StargateContext context)
        {
            _context = context;
        }
        public Task Process(CreatePerson request, CancellationToken cancellationToken)
        {
            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

            if (person != null)
            {
                //var logEntry = new CreateLogEntry
                //{
                //    LogType = "Failure",
                //    Message = $"Duplicate person,{request.Name},already exists in our records."

                //};
                //var logHandler = new CreateLogEntry();
                //var result = logHandler;
                throw new BadHttpRequestException("Bad Request");
            }
            return Task.CompletedTask;
        }
    }

    public class CreatePersonHandler : IRequestHandler<CreatePerson, CreatePersonResult>
    {
        private readonly StargateContext _context;

        public CreatePersonHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken)
        {
            var newPerson = new Person()
            {
                Name = request.Name
            };

            try
            {
                
                await _context.People.AddAsync(newPerson);

                await _context.SaveChangesAsync();

                //var logEntry = new CreateLogEntry
                //{
                //    LogType = "Success",
                //    Message = $"New Person,{newPerson}, added to the roster,."

                //};
                //var logHandler = new CreateLogEntry();
                //var result = logHandler;

                return new CreatePersonResult()
                {
                    Id = newPerson.Id
                };

            }
            catch(Exception)
            {
                //var logEntry = new CreateLogEntry
                //{
                //    LogType = "Failure",
                //    Message = $"New Person,{newPerson}, could not be added"

                //};
                //var logHandler = new CreateLogEntry();
                //var result = logHandler;
                throw new BadHttpRequestException("Bad Request"); 

            }
               
        }
    }

    public class CreatePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }
}
