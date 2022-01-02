using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.api.Libreria.Core.Entities
{
    [BsonCollection("Libro")]
    public class LibroEntity : Document
    {
        [BsonElement("titulo")]
        public string Titulo { set; get; }
        [BsonElement("descripcion")]
        public string Descripcion { set; get; }
        [BsonElement("precio")]
        public int Precio { set; get; }
        [BsonElement("fechaPublicacion")]
        public DateTime FechaPublicacion { set; get; }
        [BsonElement("autor")]
        public AutorEntity Autor { set; get; }

    }
}
