using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.api.Libreria.Core
{
    public class MongoSettings
    {
        public string ConnectionString { set; get; }
        public string Database { set; get; }
    }
}
