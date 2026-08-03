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

        public static ContactSpecs Get(string contactId)
        {
            return new ContactSpecs(p => p.ContactId == contactId);
        }

        public static ContactSpecs GetAll()
        {
            return new ContactSpecs(p => true);
        }
    }
}
