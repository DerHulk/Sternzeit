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
    public class NodeController : ControllerBase
    {
        private ITimeService TimeService { get; }
        private MongoDbContext MongoDbContext { get; }

        public NodeController(ITimeService timeService, MongoDbContext mongoDbContext)
        {
            this.TimeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            this.MongoDbContext = mongoDbContext ?? throw new ArgumentNullException(nameof(mongoDbContext));
        }        

        [HttpPut(Name= Constants.Routes.CreateNote)]
        public async Task<ActionResult<NoteModel>> Create([FromBody] string titel)
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
            var model = this.GetModelFromState(state);

            return this.CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }

        [HttpGet]
        public async Task<ActionResult<NoteModel>> Get(NoteIdModel id) 
        {
            var state = await (await this.MongoDbContext.Notes.FindAsync(x => x.Id == id.Value)).SingleAsync();
            
            var model = new NoteModel() { 
             Id = id,
             Tags = null,
             Text = state.Text,
            };

            return model;
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

            return this.NoContent();
        }

        private NoteModel GetModelFromState(NoteStates state)
        {
            var model = new NoteModel()
            {
                Id = new NoteIdModel() { Value = state.Id },
                Tags = new string[] { },
                Text = state.Text,
                Titel = state.Titel,
            };

            return model;
        }
    }
}
