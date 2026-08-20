using QueryRouting;
using CrudDatastore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Specifications
{
    public class ContactSpecs : Specification<Entities.Contact>
    {
        private ContactSpecs(Expression<Func<Entities.Contact, bool>> predicate)
            : base(predicate)
        { }

        [QueryRoute("GET", "/api/query/contacts/{id}", QueryRouteResultKind.Single)]
        public static ContactSpecs Get(string contactId)
        {
            return new ContactSpecs(p => p.ContactId == contactId);
        }

        [QueryRoute("GET", "/api/query/contacts", QueryRouteResultKind.Collection)]
        public static ContactSpecs GetAll()
        {
            return new ContactSpecs(p => true);
        }
    }
}
