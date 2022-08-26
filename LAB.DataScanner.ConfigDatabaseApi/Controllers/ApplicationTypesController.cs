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
    public class ApplicationTypesController : BaseController<ApplicationType>
    {
        public ApplicationTypesController(IRepository<ApplicationType> repo) : base(repo) { }

        [HttpGet("{id}")]
        [EnableQuery]
        public SingleResult Get(int id)
        {
            var result = _repo.ReadApplicationTypeById(id);
            if (result != null) result.ApplicationInstance = null;

            return SingleResult.Create(new[] { result }.AsQueryable());
        }
            

        [HttpPost]
        [EnableQuery]
        public IActionResult Post(ApplicationType item)
        {
            _repo.CreateApplicationType(item);            
            return Created(item);
        }

        [HttpPatch]
        [EnableQuery]
        public IActionResult Patch([FromODataUri] int id, [FromBody] Delta<ApplicationType> entityDelta)
        {
            try
            {
                _repo.UpdateApplicationType(id, entityDelta.GetInstance());
            }
            catch (DbItemNotExistException e)
            {
                return NotFound(e.Message);
            }

            return Updated(entityDelta);
        }

        [HttpDelete]
        [EnableQuery]
        public IActionResult Delete([FromODataUri] int id)
        {
            try
            {
                _repo.DeleteApplicationType(id);
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
