using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.api.Libreria.Core.Entities
{
    public class Autor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { set; get; }
        [BsonElement("nombre")]
        public string Nombre { set; get; }
        [BsonElement("apellido")]
        public string Apellido { set; get; }
        [BsonElement("gradoAcademico")]
        public string GradoAcademico { set; get; }
    }
}
