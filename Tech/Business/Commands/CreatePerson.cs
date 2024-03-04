//mediatr? -- pattern
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.Diagnostics.CodeAnalysis;

namespace StargateAPI.Business.Commands
{
    [ExcludeFromCodeCoverage]
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
                var logging = new Logging()
                {
                    LogType = "BadRequest",
                    Message = $" duplicate Person, {person}, Already exists in our records.",
                    LogException = string.Empty,
                    TimeStamp = DateTime.Now,
                };
                
                _context.Logs.Add(logging);
                _context.SaveChangesAsync();

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

                var logging = new Logging()
                {
                    LogType = "Successs",
                    Message = $"New Person,{newPerson}, added to the roster. ",
                    LogException = string.Empty,
                    TimeStamp = DateTime.Now,
                };

                _context.Logs.Add(logging);
                _context.SaveChangesAsync();
                
                return new CreatePersonResult()
                {
                    Id = newPerson.Id
                };

            }
            catch(Exception ex)
            {
                var logging = new Logging()
                {
                    LogType = "Failure",
                    Message = $"New Person,{newPerson}, could not be added.",
                    LogException = ex.ToString(),
                    TimeStamp = DateTime.Now,
                };
                _context.Logs.Add(logging);
                _context.SaveChangesAsync();

                throw new BadHttpRequestException("Bad Request"); 

            }
               
        }
    }

    public class CreatePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }
}
