using CrudDatastore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    internal class MaterializationInterceptor : IMaterializationInterceptor
    {
        private Func<object, object> _interceptor;

        public MaterializationInterceptor(Func<object, object> interceptor)
        {
            _interceptor = interceptor;
        }

        public object InitializedInstance(MaterializationInterceptionData materializationData, object instance)
        {
            if (instance is EntityBase entity)
            {
                instance = _interceptor(entity);
            }

            return instance;
        }
    }
}
