using eShopAnalysis.ProductInteractionAPI.Data;
using eShopAnalysis.ProductInteractionAPI.Models;
using MongoDB.Driver;
using Polly;

namespace eShopAnalysis.ProductInteractionAPI.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly MongoDbContext _context;

        public CommentRepository(MongoDbContext context) {
            _context = context;
        }

        public async Task<Comment> AddAsync(Guid userId, Guid productBusinessKey, string commentDetail)
        {
            Comment commentToAdd = new Comment(userId, productBusinessKey, commentDetail);
            await _context.CommentCollection.InsertOneAsync(commentToAdd);

            var filter = Builders<Comment>.Filter.And(
                Builders<Comment>.Filter.Eq(c => c.UserId, userId),
                Builders<Comment>.Filter.Eq(c => c.ProductBusinessKey, productBusinessKey)
            );

            var findResult = await _context.CommentCollection.FindAsync(filter);
            Comment commentAdded = await findResult.SingleOrDefaultAsync();
            return commentAdded;
        }

        public IQueryable<Comment> GetAllAsQueryable()
        {
            IQueryable<Comment> allQueryableComments = _context.CommentCollection.AsQueryable();
            return allQueryableComments;
        }

        public async Task<Comment> GetAsync(Guid userId, Guid productBusinessKey)
        {
            Comment findResult = await _context.CommentCollection.Find(c => c.UserId == userId && c.ProductBusinessKey == productBusinessKey).SingleOrDefaultAsync();
            if (findResult == null) {
                return null;
            }
            return findResult;
        }

        public async Task<Comment> GetAsync(Guid commentId)
        {
            Comment findResult = await _context.CommentCollection.Find(c => c.CommentId == commentId).SingleOrDefaultAsync();
            if (findResult == null) {
                return null;
            }
            return findResult;
        }

        public async Task<Comment> RemoveAsync(Guid userId, Guid productBusinessKey)
        {
            var filter = Builders<Comment>.Filter.And(
                Builders<Comment>.Filter.Eq(c => c.UserId, userId),
                Builders<Comment>.Filter.Eq(c => c.ProductBusinessKey, productBusinessKey)
            );
            Comment deletedComment = await _context.CommentCollection.FindOneAndDeleteAsync(
                c => c.UserId.Equals(userId) && c.ProductBusinessKey.Equals(productBusinessKey)
            );
            return deletedComment;
        }

        public async Task<Comment> UpdateAsync(Guid userId, Guid productBusinessKey, string updatedCommentDetail)
        {
            //find the old comment , then create the new comment with the id , if we call the consturctor to create new instance
            //it will have new _id, and cause exception in FindOneAndReplaceAsync
            Comment oldComment = await _context.CommentCollection.Find(c => c.UserId == userId && c.ProductBusinessKey == productBusinessKey).SingleOrDefaultAsync();
            if (oldComment == null) {
                return null;
            }
            oldComment.CommentDetail = updatedCommentDetail;
            oldComment.DateModified = DateTime.Now;

            var filter = Builders<Comment>.Filter.And(
                Builders<Comment>.Filter.Eq(c => c.UserId, userId),
                Builders<Comment>.Filter.Eq(c => c.ProductBusinessKey, productBusinessKey)
            );
            Comment? updatedComment = await _context.CommentCollection.FindOneAndReplaceAsync(filter, oldComment);

            if (updatedComment == null)
                return null;
            return updatedComment;
        }
    }
}
