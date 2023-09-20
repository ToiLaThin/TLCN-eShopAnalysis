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
        public Catalog Add(Catalog catalog)
        {
            _context.CatalogCollection.InsertOne(catalog);
            Catalog result = _context.CatalogCollection.Find(c => c.CatalogId == catalog.CatalogId).FirstOrDefault();
            return result;
        }

        public IEnumerable<Catalog> AddRange(IEnumerable<Catalog> catalogs)
        {
            _context.CatalogCollection.InsertMany(catalogs);
            IEnumerable<Catalog> result = new List<Catalog>();
            foreach (var catalog in catalogs)
            {
                Catalog catalogInserted = _context.CatalogCollection.Find(c => c.CatalogId == catalog.CatalogId).FirstOrDefault();
                if (catalogInserted != null)
                    result.Append(catalogInserted);
                else
                    return null;
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

        public bool Remove(Guid catalogToRevId)
        {
            var result = _context.CatalogCollection.FindOneAndDelete(c => c.CatalogId == catalogToRevId);
            return result != null ? true : false;

            //var filter = Builders<Catalog>.Filter
            //        .Eq(c => c.CatalogId, catalog.CatalogId);
            //var result = _context.CatalogCollection.DeleteOne(filter);
            //if (result.DeletedCount == 0) { return false; }
            //else { return true; }
        }
        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public bool Update(Catalog newCat)
        {
            var filter = Builders<Catalog>.Filter.Eq(oldCat => oldCat.CatalogId, newCat.CatalogId);
            //var update = Builders<Catalog>.Update.Set(oldCat => oldCat, newCat); just for update one, set field by field

            var updateResult = _context.CatalogCollection.ReplaceOne(filter, newCat);
            if (updateResult.ModifiedCount > 0)
            {
                return true;
            }
            else { return false; }
        }
    }
}
