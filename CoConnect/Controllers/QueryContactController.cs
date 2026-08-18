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
        private readonly IQueryContextFactory _queryContextFactory;

        public QueryContactController(IQueryContextFactory queryContextFactory)
        {
            _queryContextFactory = queryContextFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetAll()
        {
            List<Contact> contacts;

            using (var queryContext = _queryContextFactory.CreateQueryContext())
            {
                var result = await queryContext.FindAsync(ContactSpecs.GetAll());
                contacts = result?.ToList() ?? new List<Contact>();
            }

            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetById(string id)
        {
            Contact? contact;

            using (var queryContext = _queryContextFactory.CreateQueryContext())
            {
                contact = await queryContext.FindSingleAsync(ContactSpecs.Get(id));
            }

            if (contact == null)
                return NotFound();

            return Ok(contact);
        }
    }
}
