using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Servicios.api.Libreria.Core;
using Servicios.api.Libreria.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Servicios.api.Libreria.Repository
{
    public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : IDocument
    {
        private readonly IMongoCollection<TDocument> _collection;

        public MongoRepository(IOptions<MongoSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var db = client.GetDatabase(options.Value.Database);

            _collection = db.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
        }

        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault()).CollectionName;
        }

        public async Task<IEnumerable<TDocument>> GetAll()
        {
            return await _collection.Find(a => true).ToListAsync();
        }

        public async Task<TDocument> GetById(string id)
        {
            //return await _collection.Find(a => a.Id == id).FirstOrDefaultAsync();
            var filtro = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
            return await _collection.Find(filtro).FirstOrDefaultAsync();
        }

        public async Task InsertDocument(TDocument document)
        {
            await _collection.InsertOneAsync(document);
        }

        public async Task UpdateDocument(TDocument document)
        {
            var filtro = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            await _collection.FindOneAndReplaceAsync(filtro, document);
        }

        public async Task DeleteById(string id)
        {
            var filtro = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
            await _collection.FindOneAndDeleteAsync(filtro);
        }


        public async Task<PaginationEntity<TDocument>> PaginationBy(Expression<Func<TDocument, bool>> filterExpression, PaginationEntity<TDocument> pagination)
        {
            var sort = Builders<TDocument>.Sort.Ascending(pagination.Sort);

            if (pagination.SortDirection.ToLower().Equals("desc"))
            {
                sort = Builders<TDocument>.Sort.Descending(pagination.Sort);
            }

            long totalDocuments = 0;

            if (string.IsNullOrEmpty(pagination.Filter))
            {
                pagination.Data = await _collection.Find(a => true).Sort(sort)
                    .Skip( (pagination.Page - 1) * pagination.PageSize )
                    .Limit( pagination.PageSize )
                    .ToListAsync();

                //totalDocuments = await _collection.CountDocumentsAsync(a => true);
            }
            else
            {
                pagination.Data = await _collection.Find(filterExpression).Sort(sort)
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Limit(pagination.PageSize)
                    .ToListAsync();

                //totalDocuments = await _collection.CountDocumentsAsync(filterExpression);
            }

            pagination.TotalDocuments = pagination.Data.Count();
            //pagination.PagesQuantity = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalDocuments / pagination.PageSize)));
            var cantidadTotalPaginas = Convert.ToInt32(totalDocuments / pagination.PageSize);
            pagination.PagesQuantity = (cantidadTotalPaginas == 0 && pagination.Data.Count() > 0) ? 1 : cantidadTotalPaginas;

            return pagination;
        }


        public async Task<PaginationEntity<TDocument>> PaginationByFilter(PaginationEntity<TDocument> pagination)
        {
            var sort = Builders<TDocument>.Sort.Ascending(pagination.Sort);

            if (pagination.SortDirection.ToLower().Equals("desc"))
            {
                sort = Builders<TDocument>.Sort.Descending(pagination.Sort);
            }

            long totalDocuments = 0;

            if (pagination.Filters == null)
            {
                pagination.Data = await _collection.Find(a => true).Sort(sort)
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Limit(pagination.PageSize)
                    .ToListAsync();
            }
            else
            {
                //Expresion para buscar en Mongo
                var valueFilter = ".*" + pagination.Filters.Valor + ".*";
                
                //La "i" es para que no sea case sensitive
                var filter = Builders<TDocument>.Filter.Regex(pagination.Filters.Propiedad, new BsonRegularExpression(valueFilter, "i"));

                pagination.Data = await _collection.Find(filter).Sort(sort)
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Limit(pagination.PageSize)
                    .ToListAsync();
            }

            //totalDocuments = await _collection.CountDocumentsAsync(FilterDefinition<TDocument>.Empty);
            pagination.TotalDocuments = pagination.Data.Count();
            //pagination.PagesQuantity = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalDocuments / pagination.PageSize)));
            var cantidadTotalPaginas = Convert.ToInt32(totalDocuments / pagination.PageSize);
            pagination.PagesQuantity = (cantidadTotalPaginas == 0 && pagination.Data.Count() > 0) ? 1 : cantidadTotalPaginas;

            return pagination;
        }
    }
}
