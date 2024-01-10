using eShopAnalysis.ProductInteractionAPI.Data;
using eShopAnalysis.ProductInteractionAPI.Models;
using MongoDB.Driver;
using Polly;

namespace eShopAnalysis.ProductInteractionAPI.Repository
{
    //like is a mapping instance showing that user with userId liked product with product lusiness key
    public class LikeRepository : ILikeRepository
    {
        private readonly MongoDbContext _context;

        public LikeRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<Like> AddAsync(Guid userId, Guid productBusinessKey, LikeStatus status)
        {
            Like likedMappingToAdd = new Like(userId, productBusinessKey, status);
            await _context.LikeCollection.InsertOneAsync(likedMappingToAdd);

            var filter = Builders<Like>.Filter.And(
                Builders<Like>.Filter.Eq(l => l.UserId, userId),
                Builders<Like>.Filter.Eq(l => l.ProductBusinessKey, productBusinessKey)
            );

            var findResult = await _context.LikeCollection.FindAsync(filter);
            Like likedMappingAdded = await findResult.SingleOrDefaultAsync();
            return likedMappingAdded;
        }

        public IQueryable<Like> GetAllAsQueryable()
        {
            IQueryable<Like> allQueryalleLikedMappings = _context.LikeCollection.AsQueryable();
            return allQueryalleLikedMappings;
        }

        public async Task<Like> GetAsync(Guid userId, Guid productBusinessKey)
        {
            Like findResult = await _context.LikeCollection.Find(l => l.UserId == userId && l.ProductBusinessKey == productBusinessKey).SingleOrDefaultAsync();
            if (findResult == null)
            {
                return null;
            }
            return findResult;
        }

        public async Task<Like> GetAsync(Guid likeId)
        {
            Like findResult = await _context.LikeCollection.Find(l => l.LikeId == likeId).SingleOrDefaultAsync();
            if (findResult == null)
            {
                return null;
            }
            return findResult;
        }

        //public async Task<IEnumerable<Like>> GetLikedOfUserAsync(Guid userId)
        //{
        //    var filterUser = Builders<Like>.Filter.Eq(l => l.UserId, userId);

        //    var findResult = await _context.LikeCollection.FindAsync(filterUser);
        //    IEnumerable<Like> userLikedMappings = await findResult.ToListAsync();
        //    return userLikedMappings;
        //}

        public async Task<Like> RemoveAsync(Guid userId, Guid productBusinessKey)
        {
            var filter = Builders<Like>.Filter.And(
                            Builders<Like>.Filter.Eq(l => l.UserId, userId),
                            Builders<Like>.Filter.Eq(l => l.ProductBusinessKey, productBusinessKey)
                        );
            Like deletedLikedMapping = await _context.LikeCollection.FindOneAndDeleteAsync(
                l => l.UserId.Equals(userId) && l.ProductBusinessKey.Equals(productBusinessKey)
            );
            return deletedLikedMapping; //null or the liked mapping deleted
        }

        public async Task<Like> UpdateAsync(Guid userId, Guid productBusinessKey, LikeStatus updatedLikeStatus)
        {            
            Like oldLikeMapping = await _context.LikeCollection.Find(l => l.UserId.Equals(userId) && l.ProductBusinessKey.Equals(productBusinessKey))
                                                               .SingleOrDefaultAsync();
            if (oldLikeMapping == null) {
                return null;
            }

            oldLikeMapping.Status = updatedLikeStatus;
            var filter = Builders<Like>.Filter.And(
               Builders<Like>.Filter.Eq(l => l.UserId, userId),
               Builders<Like>.Filter.Eq(l => l.ProductBusinessKey, productBusinessKey)
            );
            Like? updatedLikeMapping = await _context.LikeCollection.FindOneAndReplaceAsync(filter, oldLikeMapping);

            if (updatedLikeMapping == null) {
                return null;
            }
            return updatedLikeMapping;
        }
    }
}
