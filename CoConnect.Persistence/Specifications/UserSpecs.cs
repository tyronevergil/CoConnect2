using System.Linq.Expressions;
using QueryRouting;
using CrudDatastore;

namespace CoConnect.Persistence.Specifications
{
    public class UserSpecs : Specification<Entities.User>
    {
        private UserSpecs(Expression<Func<Entities.User, bool>> predicate)
            : base(predicate)
        {
        }

        [QueryRoute("GET", "/api/query/users/{id}", QueryRouteResultKind.Single)]
        public static UserSpecs Get(string userId)
        {
            return new UserSpecs(p => p.UserId == userId);
        }

        [QueryRoute("GET", "/api/query/users", QueryRouteResultKind.Collection)]
        public static UserSpecs GetAll()
        {
            return new UserSpecs(p => true);
        }

        public static UserSpecs GetByUsername(string username)
        {
            return new UserSpecs(p => p.Username == username);
        }
    }
}

