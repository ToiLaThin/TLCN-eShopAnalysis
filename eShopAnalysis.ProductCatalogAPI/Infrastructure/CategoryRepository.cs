using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Infrastructure.Data;
using MongoDB.Driver;
using SharpCompress.Common;
using System;
using System.Linq.Expressions;

namespace eShopAnalysis.ProductCatalogAPI.Infrastructure
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly MongoDbContext _context;

        public CatalogRepository(MongoDbContext context)
        {
            _context = context;
        }

        //if adding with transaction, find will return null, because the record not really saved to db
        //so if used this with trans, change it to void and do not find the inserted one
        public Catalog Add(Catalog catalog, IClientSessionHandle sessionHandle = null)
        {
            bool sessionIsNull = sessionHandle == null;
            if (!sessionIsNull && !sessionHandle.IsInTransaction)
                throw new InvalidOperationException("used not correctly");

            if (sessionIsNull) {
                _context.CatalogCollection.InsertOne(catalog);
            }
            _context.CatalogCollection.InsertOne(session: sessionHandle ,catalog);

            Catalog result = _context.CatalogCollection.Find(c => c.CatalogId == catalog.CatalogId).FirstOrDefault();
            return result;

        }

        //if adding with transaction, find will return null, because the record not really saved to db
        //so if used this with trans, change it to void and do not find the inserted one
        public IEnumerable<Catalog> AddRange(IEnumerable<Catalog> catalogs, IClientSessionHandle sessionHandle = null)
        {
            bool sessionIsNull = sessionHandle == null;
            if (!sessionIsNull && !sessionHandle.IsInTransaction)
                throw new InvalidOperationException("used not correctly");

            if (sessionIsNull) {
                _context.CatalogCollection.InsertMany(catalogs);
            }
            _context.CatalogCollection.InsertMany(session: sessionHandle ,catalogs);
            
            IEnumerable<Catalog> result = new List<Catalog>();
            foreach (var catalog in catalogs) {
                Catalog catalogInserted = _context.CatalogCollection.Find(c => c.CatalogId == catalog.CatalogId).FirstOrDefault();
                if (catalogInserted == null) {
                    return null;
                }
                result.Append(catalogInserted);
            }
            return result;
        }

        public IEnumerable<Catalog> Find(Expression<Func<Catalog, bool>> predicate)
        {
            IEnumerable<Catalog> catalogs = _context.CatalogCollection.AsQueryable().Where(predicate).ToList();
            return catalogs;
        }

        public Catalog Get(Guid id)
        {
            Catalog result = _context.CatalogCollection.AsQueryable().Where(c => c.CatalogId == id).FirstOrDefault();
            return result;
        }

        public IEnumerable<Catalog> GetAll()
        {
            var result = _context.CatalogCollection.AsQueryable().ToList();
            return result;
        }

        public IQueryable<Catalog> GetAllAsQueryable()
        {
            var result = _context.CatalogCollection.AsQueryable();
            return result;
        }

        public Catalog GetFirstEntity()
        {
            var result = _context.CatalogCollection.AsQueryable().FirstOrDefault();
            return result;
        }

        public bool Remove(Guid catalogToRevId, IClientSessionHandle sessionHandle = null)
        {
            bool sessionIsNull = sessionHandle == null;
            if (!sessionIsNull && !sessionHandle.IsInTransaction)
                throw new InvalidOperationException("used not correctly");

            var result = sessionIsNull ? _context.CatalogCollection.FindOneAndDelete(
                                                c => c.CatalogId == catalogToRevId) :
                                         _context.CatalogCollection.FindOneAndDelete(session: sessionHandle, 
                                                c => c.CatalogId == catalogToRevId);
            return result != null ? true : false;

            //var filter = Builders<Catalog>.Filter
            //        .Eq(c => c.CatalogId, catalog.CatalogId);
            //var result = _context.CatalogCollection.DeleteOne(filter);
            //if (result.DeletedCount == 0) { return false; }
            //else { return true; }
        }

        public void SaveChanges(IClientSessionHandle sessionHandle = null)
        {
            throw new NotImplementedException();
        }

        public bool Update(Catalog newCat, IClientSessionHandle sessionHandle = null)
        {
            bool sessionIsNull = sessionHandle == null;
            if (!sessionIsNull && !sessionHandle.IsInTransaction)
                throw new InvalidOperationException("used not correctly");

            var filter = Builders<Catalog>.Filter.Eq(oldCat => oldCat.CatalogId, newCat.CatalogId);
            //var update = Builders<Catalog>.Update.Set(oldCat => oldCat, newCat); just for update one, set field by field

            var updateResult = sessionIsNull ? _context.CatalogCollection.ReplaceOne(filter, newCat) :
                                                _context.CatalogCollection.ReplaceOne(session: sessionHandle, filter, newCat);
            if (updateResult.ModifiedCount > 0) {
                return true;
            }
            else { return false; }
        }
    }
}
