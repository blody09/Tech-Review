using Dapper;
using MediatR;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;
using System;

namespace StargateAPI.Business.Queries
{
    public class GetLogs : IRequest<GetLogsResult>
    {

    }

    public class GetLogsHandler : IRequestHandler<GetLogs, GetLogsResult>
    {
        private readonly StargateContext _context;
        public GetLogsHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<GetLogsResult> Handle(GetLogs request, CancellationToken cancellationToken)
        {
            var result = new GetLogsResult();
            var query = $"SELECT * FROM [Logging]";
            var logs = await _context.Connection.QueryAsync<Logging>(query);
            result.LogsResultList = logs.ToList();
            return result;
        }
    }
    public class GetLogsResult : BaseResponse
    {
        public List<Logging> LogsResultList { get; set; } = new List<Logging> { };

    }

}
