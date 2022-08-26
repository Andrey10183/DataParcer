using LAB.DataScanner.ConfigDatabaseApi.Interfaces;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;

namespace LAB.DataScanner.ConfigDatabaseApi.Controllers
{
    public abstract class BaseController<T> : ODataController
        where T : class
    {
        protected readonly IRepository<T> _repo;

        protected BaseController(IRepository<T> repo) =>
            _repo = repo;

        [HttpGet]
        [EnableQuery()]
        public virtual IActionResult Get() =>
            Ok(_repo.ReadAll());
    }
}
