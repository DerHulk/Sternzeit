using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sternzeit.Server.Tests.Helpers
{
    public class MongoDbContextMock : MongoDbContext
    {
        public Mock<IMongoDatabase> MockDB { get; }
        private Dictionary<string, object> CollectionMockMap { get; }

        public static MongoDbContextMock Create()
        {
            var mongoMock = new Mock<IMongoDatabase>();            
            return new MongoDbContextMock(mongoMock);
        }

        public MongoDbContextMock(Mock<IMongoDatabase> mockDB) : base(mockDB.Object)
        {
            this.MockDB = mockDB ?? throw new ArgumentNullException(nameof(mockDB));
            this.CollectionMockMap = new Dictionary<string, object>();
        }

        public Mock<IMongoCollection<T>> CreateMockCollection<T>(string name)
        {
            var mock = new Mock<IMongoCollection<T>>();
            this.MockDB.Setup(x => x.GetCollection<T>(It.Is<string>(i => i == name), It.IsAny<MongoCollectionSettings>())).Returns(mock.Object);

            this.CollectionMockMap.Add(name, mock);

            return mock;
        }

        public Mock<IMongoCollection<T>> GetMockCollection<T>(string name)
        {
            var mock = this.CollectionMockMap[name];
            return (Mock<IMongoCollection<T>>)mock;
        }
    }
}
