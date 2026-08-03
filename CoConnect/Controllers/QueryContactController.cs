using Microsoft.AspNetCore.Mvc;
using Persistence;
using Persistence.Entities;
using Persistence.Specifications;

namespace CoConnect.Controllers
{
    [ApiController]
    [Route("api/query/contacts")]
    public class QueryContactController : ControllerBase
    {
        private readonly IDataContextFactory _dataContextFactory;

        public QueryContactController(IDataContextFactory dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetAll()
        {
            List<Contact> contacts;

            using (var dataContext = _dataContextFactory.CreateDataContext())
            {
                var result = await dataContext.FindAsync(ContactSpecs.GetAll());
                contacts = result?.ToList() ?? new List<Contact>();
            }

            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetById(string id)
        {
            Contact? contact;

            using (var dataContext = _dataContextFactory.CreateDataContext())
            {
                contact = await dataContext.FindSingleAsync(ContactSpecs.Get(id));
            }

            if (contact == null)
                return NotFound();

            return Ok(contact);
        }
    }
}
