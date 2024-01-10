using eShopAnalysis.ProductInteractionAPI.Data;
using eShopAnalysis.ProductInteractionAPI.Models;
using MongoDB.Driver;

namespace eShopAnalysis.ProductInteractionAPI.Repository
{
    public class BookmarkRepository : IBookmarkRepository
    {
        private readonly MongoDbContext _context;

        public BookmarkRepository(MongoDbContext context) {
            _context = context;
        }

        public async Task<Bookmark> AddAsync(Guid userId, Guid productBusinessKey)
        {
            Bookmark bookmarkToAdd = new Bookmark(userId, productBusinessKey);
            await _context.BookmarkCollection.InsertOneAsync(bookmarkToAdd);

            var filter = Builders<Bookmark>.Filter.And(
                Builders<Bookmark>.Filter.Eq(b => b.UserId, userId),
                Builders<Bookmark>.Filter.Eq(b => b.ProductBusinessKey, productBusinessKey)
            );

            var findResult = await _context.BookmarkCollection.FindAsync(filter);
            Bookmark bookmarkAdded = await findResult.SingleOrDefaultAsync();
            return bookmarkAdded;

        }

        public IQueryable<Bookmark> GetAllAsQueryable()
        {
            //just for testing
            IQueryable<Bookmark> allQueryableBookmarks = _context.BookmarkCollection.AsQueryable();
            return allQueryableBookmarks;
        }

        public async Task<Bookmark> GetAsync(Guid userId, Guid productBusinessKey)
        {
            Bookmark findResult = await _context.BookmarkCollection.Find(b => b.UserId == userId &&  b.ProductBusinessKey == productBusinessKey).SingleOrDefaultAsync();
            if (findResult == null) {
                return null;
            }
            return findResult;
        }

        public async Task<Bookmark> GetAsync(Guid bookmarkId)
        {
            Bookmark findResult = await _context.BookmarkCollection.Find(b => b.BookmarkId == bookmarkId).SingleOrDefaultAsync();
            if (findResult == null) {
                return null;
            }
            return findResult;
        }

        //public async Task<IEnumerable<Bookmark>> GetBookmarksOfUserAsync(Guid userId)
        //{
        //    var filterUser = Builders<Bookmark>.Filter.Eq(b => b.UserId, userId);

        //    var findResult = await _context.BookmarkCollection.FindAsync(filterUser);
        //    IEnumerable<Bookmark> userBookmarks = await findResult.ToListAsync();
        //    return userBookmarks;
        //}

        public async Task<Bookmark> RemoveAsync(Guid userId, Guid productBusinessKey)
        {
            var filter = Builders<Bookmark>.Filter.And(
                Builders<Bookmark>.Filter.Eq(b => b.UserId, userId),
                Builders<Bookmark>.Filter.Eq(b => b.ProductBusinessKey, productBusinessKey)
            );
            Bookmark deletedUserBookmark = await _context.BookmarkCollection.FindOneAndDeleteAsync(
                b => b.UserId.Equals(userId) && b.ProductBusinessKey.Equals(productBusinessKey)
            );
            return deletedUserBookmark; //null or the bookmark deleted

        }
    }
}
