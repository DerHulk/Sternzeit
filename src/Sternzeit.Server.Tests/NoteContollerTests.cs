using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using Sternzeit.Server.Controllers;
using Sternzeit.Server.Models;
using Sternzeit.Server.Services;
using Sternzeit.Server.States;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sternzeit.Server.Tests
{
    public class NoteContollerTests
    {
        private NoteController Target { get; }
        private Mock<ITimeService> TimeService { get; }
        private Mock<IUserService> UserService { get; }
        private Helpers.MongoDbContextMock MongoDb { get; }
        

        public NoteContollerTests()
        {
            this.TimeService = new Mock<ITimeService>();
            this.UserService = new Mock<IUserService>();
            this.MongoDb = Helpers.MongoDbContextMock.Create();
            this.Target = new NoteController(this.TimeService.Object, this.UserService.Object, this.MongoDb);
        }

        [Fact(DisplayName ="Check that a valid note will be created with ok.")]
        public async Task Create01()
        {
            //arrange
            var titel = "test";
            var collection = this.MongoDb.CreateMockCollection<NoteStates>("Notes");

            this.UserService.Setup(x => x.GetCurrentUserId()).ReturnsAsync(Guid.Empty);
            
            //act
            var result = await this.Target.Create(titel);

            //assert
            Assert.IsType<ActionResult<NoteModel>>(result);
            collection.Verify(x => x.InsertOneAsync(It.Is<NoteStates>(x => x.Titel == titel), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()));
        }

        [Theory(DisplayName = "Check that no content will be returned if the titel is null/empty.")]
        [InlineData("")]
        [InlineData(null)]
        public async Task Create02(string titel)
        {
            //arrange
            this.MongoDb.CreateMockCollection<NoteStates>("Notes");

            //act
            var result = await this.Target.Create(titel);

            //assert
            Assert.IsType<ActionResult<NoteModel>>(result);
            Assert.IsType<NoContentResult>(result.Result);
        }

    }
}
