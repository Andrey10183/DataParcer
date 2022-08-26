using LAB.DataScanner.ConfigDatabaseApi.Infrastructure;
using LAB.DataScanner.ConfigDatabaseApi.Interfaces;
using LAB.DataScanner.ConfigDatabaseApi.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LAB.DataScanner.ConfigDatabaseApi.Controllers
{
    [Route("odata/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class ApplicationInstancesController : BaseController<ApplicationInstance>
    {
        public ApplicationInstancesController(IRepository<ApplicationInstance> repo) : base(repo) { }

        [HttpGet("{id}")]
        [EnableQuery]
        public SingleResult Get(int id)
        {
            var result = _repo.ReadApplicationInstancesById(id);
            if (result != null)
            {
                result.BindingConsumerInstances = null;
                result.BindingPublisherInstances = null;
            } 
            return SingleResult.Create(new[] { result }.AsQueryable());
        }
            
        [HttpPost]
        [EnableQuery]
        public IActionResult Post(ApplicationInstance item)
        {
            try
            {
                _repo.CreateApplicationInstance(item);
            }
            catch (DbItemNotExistException e)
            {
                return NotFound(e.Message);
            }

            return Created(item);
        }

        [HttpPatch]
        [EnableQuery]
        public IActionResult Patch(int id, [FromBody] Delta<ApplicationInstance> entityDelta)
        {
            ApplicationInstance item;
            try
            {
                item = entityDelta.GetInstance();
                _repo.UpdateApplicationInstance(id, entityDelta.GetInstance());
            }
            catch (DbItemNotExistException e)
            {
                return NotFound(e.Message);
            }

            return Updated(item);           
        }

        [HttpDelete]
        public IActionResult Delete([FromODataUri] int id)
        {
            try
            {
                _repo.DeleteApplicationInstance(id);
            }
            catch (DbItemNotExistException e)
            {
                return NotFound(e.Message);
            }
            catch (DbEntityBoundedException e)
            {
                return Conflict(e.Message);
            }

            return NoContent();
        }
    }
}
