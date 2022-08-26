using LAB.DataScanner.ConfigDatabaseApi.Infrastructure;
using LAB.DataScanner.ConfigDatabaseApi.Interfaces;
using LAB.DataScanner.ConfigDatabaseApi.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace LAB.DataScanner.ConfigDatabaseApi.Controllers
{
    [Route("odata/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class BindingController : BaseController<Binding>
    {
        public BindingController(IRepository<Binding> repo) : base(repo) { }

        [HttpGet("({keyPublisherInstanceID},{keyConsumerInstanceID})")]
        [EnableQuery]
        public SingleResult Get(
            int keyPublisherInstanceID,
            int keyConsumerInstanceID)
        {
            var result = _repo.ReadBindingsByKeys(keyPublisherInstanceID, keyConsumerInstanceID);
            return SingleResult.Create(new[] { result }.AsQueryable());
        }

        [HttpPost]
        [EnableQuery]
        public IActionResult Post(Binding item)
        {
            try
            {
                _repo.CreateBinding(item);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (DbItemNotExistException e)
            {
                return NotFound(e.Message);
            }
            catch (DbItemAlreadyExist e)
            {
                return Conflict(e.Message);
            }

            return Content("Binding created!");
        }

        [HttpDelete]
        [EnableQuery]
        public IActionResult Delete(
            int keyPublisherInstanceID,
            int keyConsumerInstanceID)
        {
            try
            {
                _repo.DeleteBinding(keyPublisherInstanceID, keyConsumerInstanceID);
            }
            catch (DbItemNotExistException e)
            {
                return NotFound(e.Message);
            }

            return NoContent();
        }
    }
}
