using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sternzeit.Server.Models;
using Sternzeit.Server.Services;
using Sternzeit.Server.States;
using MongoDB.Driver;

namespace Sternzeit.Server.Controllers
{
    [Authorize]
    [Route("Node")]
    public class NodeController : Controller
    {
        private ITimeService TimeService { get; }
        private MongoDbContext MongoDbContext { get; }

        public NodeController(ITimeService timeService, MongoDbContext mongoDbContext)
        {
            this.TimeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            this.MongoDbContext = mongoDbContext ?? throw new ArgumentNullException(nameof(mongoDbContext));
        }        

        [HttpPut]
        public async Task<ActionResult> Create(string titel)
        {
            if (string.IsNullOrEmpty(titel))
                return this.NoContent();
            
            var state = new NoteStates() { 
             Id = Guid.NewGuid(),
             CreatedAt = this.TimeService.Now(),
             OwnerId = Guid.Empty,
             Tags = new string[] { },
             Text = string.Empty,
             Color = null,
             Titel = titel,
            };

            await this.MongoDbContext.Notes.InsertOneAsync(state);

            return this.Ok();//return url to ressource;
        }

        [HttpPatch]
        public ActionResult Edit(NoteModel model)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(NoteIdModel model)
        {
            if (this.ModelState.IsValid)
                return this.ValidationProblem();
            
            await this.MongoDbContext.Notes.DeleteOneAsync(x => x.Id == model.Value);

            return this.Ok();
        }
    }
}
