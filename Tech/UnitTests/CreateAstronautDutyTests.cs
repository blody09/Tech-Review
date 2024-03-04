using NUnit.Framework;
using Moq;
using StargateAPI.Business.Data;
using StargateAPI.Business.Commands;
using Microsoft.EntityFrameworkCore;
using Dapper;
namespace StargateAPI.UnitTests
{
    [TestFixture]
    public class CreateAstronautDutyTests
    {
        
        private Mock<StargateContext> _contextMock;
        private CreateAstronautDutyHandler _handlerMock;

        [SetUp]
        public void Setup()
        {
            _contextMock = new Mock<StargateContext>(new DbContextOptions<StargateContext>());
            _handlerMock = new CreateAstronautDutyHandler(_contextMock.Object);
           
        }

        [Test]
        public async Task Valid_CreateAstronautDutyResult()
        {
            //build test things here
            var request = new CreateAstronautDuty
            {
                Name = "John",
                Rank = "Sergeant",
                DutyTitle = "Navigator",
                DutyStartDate = DateTime.Now,
            };

            var person = new Person { Id = 1, Name = request.Name };
            AstronautDetail astronautDetail = null; 
            AstronautDuty astronautDuty = null;

            //setup queries and details
            
            _contextMock.Setup(x => x.Connection.QueryFirstOrDefaultAsync<Person>(It.IsAny<string>())).ReturnsAsync(person);



        }

    }
}
