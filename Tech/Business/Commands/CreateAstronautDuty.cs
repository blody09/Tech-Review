﻿using Dapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.Net;
using System.Linq;
using StargateAPI.Business.Queries;
using System.Diagnostics.CodeAnalysis; // do i want to switch to linq quieries for better security --if time permits?


namespace StargateAPI.Business.Commands
{
    [ExcludeFromCodeCoverage]
    public class CreateAstronautDuty : IRequest<CreateAstronautDutyResult>
    {
        public required string Name { get; set; }

        public required string Rank { get; set; }

        public required string DutyTitle { get; set; }

        public DateTime DutyStartDate { get; set; }
    }

    public class CreateAstronautDutyPreProcessor : IRequestPreProcessor<CreateAstronautDuty>
    {
        private readonly StargateContext _context;
       
        public CreateAstronautDutyPreProcessor(StargateContext context)
        {
            _context = context;
          
        }

        public Task Process(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

            if (person == null)
            {
                var logging = new Logging()
                {
                    LogType = "BadRequest",
                    Message = $"Person, {person.Name}, doesnt exist or does not have a astronaut service record .",
                    LogException = string.Empty,
                    TimeStamp = DateTime.Now,
                };
                //write to db connection
                _context.Logs.Add(logging);
                _context.SaveChangesAsync();

                throw new BadHttpRequestException("Bad Request");
                
            }
           
            var verifyNoPreviousDuty = _context.AstronautDuties.FirstOrDefault(z => z.DutyTitle == request.DutyTitle && z.DutyStartDate == request.DutyStartDate);

            if (verifyNoPreviousDuty != null)
            {
                var logging = new Logging()  ///real world with time permitting this would be CreatLogEntry and have a converter for line 68 & 69  so that it could be async
                {
                    LogType = "Failure",
                    Message = $"Astronaut,{person.Name}, already has assigned duties,{request.DutyTitle}, assigned on {request.DutyStartDate}.",
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

    public class CreateAstronautDutyHandler : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
    {
        private readonly StargateContext _context;

        public CreateAstronautDutyHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<CreateAstronautDutyResult> Handle(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            try
            {
                var query = $"SELECT * FROM [Person] WHERE \'{request.Name}\' = Name";

                var person = await _context.Connection.QueryFirstOrDefaultAsync<Person>(query);

                query = $"SELECT * FROM [AstronautDetail] WHERE {person.Id} = PersonId";

                var astronautDetail = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDetail>(query);
                
                //adding to see if person has records that already exists 

                var query2 = $"SELECT * FROM [AstronautDetail] WHERE {person.Id} = {person.Name}";
                
                if (astronautDetail != null)
                {
                    var logging1 = new Logging()
                    {
                        LogType = "Bad Request",
                        Message = $"Person, {person.Name}, already has Astronaut Records",
                        LogException = String.Empty,
                        TimeStamp = DateTime.Now,
                    };
                    _context.Logs.Add(logging1);
                    _context.SaveChangesAsync();

                    throw new BadHttpRequestException("Bad Request");

                }

                if (astronautDetail == null)
                {
                    astronautDetail = new AstronautDetail();
                    astronautDetail.PersonId = person.Id;
                    astronautDetail.CurrentDutyTitle = request.DutyTitle;
                    astronautDetail.CurrentRank = request.Rank;
                    astronautDetail.CareerStartDate = request.DutyStartDate.Date;
                    if (request.DutyTitle == "RETIRED")
                    {
                        astronautDetail.CareerEndDate = request.DutyStartDate.Date;
                    }

                    await _context.AstronautDetails.AddAsync(astronautDetail);
                   
                }
                else
                {
                    astronautDetail.CurrentDutyTitle = request.DutyTitle;
                    astronautDetail.CurrentRank = request.Rank;
                    if (request.DutyTitle == "RETIRED")
                    {
                        astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                    }
                    _context.AstronautDetails.Update(astronautDetail);
                }

                query = $"SELECT * FROM [AstronautDuty] WHERE {person.Id} = PersonId Order By DutyStartDate Desc";

                var astronautDuty = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDuty>(query);

                if (astronautDuty != null)
                {
                    astronautDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
                    _context.AstronautDuties.Update(astronautDuty);
                }

                var newAstronautDuty = new AstronautDuty()
                {
                    PersonId = person.Id,
                    Rank = request.Rank,
                    DutyTitle = request.DutyTitle,
                    DutyStartDate = request.DutyStartDate.Date,
                    DutyEndDate = null
                };

                await _context.AstronautDuties.AddAsync(newAstronautDuty);
                
                await _context.SaveChangesAsync();

                var logging = new Logging()  
                {
                    LogType = "Successs",
                    Message = $"Astronaut duty created for ,{person.Name}, Duty Title: ,{request.DutyTitle}, Starting on: ,{request.DutyStartDate}. ",
                    LogException = string.Empty,
                    TimeStamp = DateTime.Now,
                };

                _context.Logs.Add(logging);
                _context.SaveChangesAsync();

                return new CreateAstronautDutyResult()
                {
                    Id = newAstronautDuty.Id
                };
            }
            catch(Exception ex)
            {
                var logging = new Logging()  
                {
                    LogType = "Bad Request",
                    Message = $"Astronaut,{request.Name}, already has assigned duties,{request.DutyTitle}, assigned on {request.DutyStartDate}.",
                    LogException = ex.ToString(),
                    TimeStamp = DateTime.Now,
                };
                _context.Logs.Add(logging);
                _context.SaveChangesAsync();

                throw new BadHttpRequestException("Bad Request");
            }
            
        }
    }

    public class CreateAstronautDutyResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}
