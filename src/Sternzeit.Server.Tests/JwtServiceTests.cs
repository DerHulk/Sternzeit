using Microsoft.Extensions.Configuration;
using Moq;
using Sternzeit.Server.Services;
using Sternzeit.Server.Services.Jwt;
using System;
using System.Collections.Generic;
using Xunit;

namespace Sternzeit.Server.Tests
{
    public class Tests
    {
        private JwtService Target { get; }
        private Mock<ITimeService> TimeService { get; }
        private IConfiguration Configuration { get; }
        private Dictionary<string, string> ConfigurationList { get; }

        public Tests()
        {
            this.ConfigurationList = new Dictionary<string, string>();
            this.Configuration = new ConfigurationBuilder().AddInMemoryCollection(this.ConfigurationList).Build();
            this.TimeService = new Mock<ITimeService>();
            this.Target = new JwtService(this.Configuration, this.TimeService.Object);
        }

        [Fact]
        public void Test1()
        {
            //arrange

            //act 
            Action action = () => this.Target.CreateToken(null);

            //assert
            Assert.Throws<ArgumentNullException>(action);
        }
    }
}