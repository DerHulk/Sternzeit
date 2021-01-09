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
    [Route("Note")]
    public class NoteController : ControllerBase
    {
        private ITimeService TimeService { get; }
        public IUserService UserService { get; }
        private MongoDbContext MongoDbContext { get; }

        public NoteController(ITimeService timeService, IUserService userService, MongoDbContext mongoDbContext)
        {
            this.TimeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
            this.UserService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.MongoDbContext = mongoDbContext ?? throw new ArgumentNullException(nameof(mongoDbContext));
        }        

        [HttpPut(Name= Constants.Routes.CreateNote)]
        public async Task<ActionResult<NoteModel>> Create([FromBody] string titel)
        {
            if (string.IsNullOrEmpty(titel))
                return this.NoContent();

            var userId = await this.UserService.GetCurrentUserId();

            if (!userId.HasValue)
                return this.BadRequest();

            var state = new NoteStates() { 
             Id = Guid.NewGuid(),
             CreatedAt = this.TimeService.Now(),
             OwnerId = userId.Value,
             Tags = new string[] { },
             Text = string.Empty,
             Color = null,
             Titel = titel,
            };
            
            await this.MongoDbContext.Notes.InsertOneAsync(state);
            var model = this.GetModelFromState(state);

            return this.CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }

        [HttpGet(Name = Constants.Routes.GetNote)]
        public async Task<ActionResult<NoteModel>> Get([FromRoute] NoteIdModel id)
        {
            var state = await (await this.MongoDbContext.Notes.FindAsync(x => x.Id == id.Value)).SingleAsync();

            var model = new NoteModel()
            {
                Id = id,
                Tags = null,
                Titel = state.Titel,
                Text = state.Text,
            };

            return model;
        }

        [HttpPatch]
        public async Task<ActionResult> Edit([FromBody]NoteModel model)
        {
            var state = await (await this.MongoDbContext.Notes.FindAsync(x => x.Id == model.Id.Value)).SingleAsync();

            state.Titel = model.Titel;
            state.Text = model.Text;
            state.LastModifiedAt = this.TimeService.Now();

            await this.MongoDbContext.Notes.ReplaceOneAsync(x => x.Id == model.Id.Value, state);

            return this.NoContent();            
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
