using Microsoft.AspNetCore.Mvc;
using Moq;
using Sternzeit.Server.Controllers;
using Sternzeit.Server.Services;
using Sternzeit.Server.States;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sternzeit.Server.Tests
{
    public class NodeContollerTests
    {
        private NodeController Target { get; }
        private Mock<ITimeService> TimeService { get; }
        private Helpers.MongoDbContextMock MongoDb { get; }

        public NodeContollerTests()
        {
            this.TimeService = new Mock<ITimeService>();
            this.MongoDb = Helpers.MongoDbContextMock.Create();
            this.Target = new NodeController(this.TimeService.Object, this.MongoDb);
        }

        [Fact(DisplayName ="Check that a valid note will be created.")]
        public async Task Create01()
        {
            //arrange
            this.MongoDb.CreateMockCollection<NoteStates>("Notes");

            //act
            var result = await this.Target.Create("test");

            //assert
            Assert.IsType<OkResult>(result);            
        }

    }
}
