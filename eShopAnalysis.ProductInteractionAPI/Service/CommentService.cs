
using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;
using eShopAnalysis.ProductInteractionAPI.Repository;
using MongoDB.Driver.Linq;

namespace eShopAnalysis.ProductInteractionAPI.Service
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<ServiceResponseDto<Comment>> Add(Guid userId, Guid productBusinessKey, string commentDetail)
        {
            bool commentExisted = await _commentRepository.GetAsync(userId, productBusinessKey) != null;
            if (commentExisted == true)
            {
                return ServiceResponseDto<Comment>.Failure("Cannot added comment because user already comment this product");
            }

            //TODO we can add logic that check the validity of commentDetail here
            var commentAdded = await _commentRepository.AddAsync(userId, productBusinessKey, commentDetail);
            if (commentAdded == null)
            {
                return ServiceResponseDto<Comment>.Failure("Cannot added comment because cannot find it");
            }

            return ServiceResponseDto<Comment>.Success(commentAdded);
        }

        public async Task<ServiceResponseDto<Comment>> Get(Guid userId, Guid productBusinessKey)
        {
            Comment commentToFind = await _commentRepository.GetAsync(userId, productBusinessKey);
            if (commentToFind == null) {
                return ServiceResponseDto<Comment>.Failure("Cannot find user comment about product");
            }
            return ServiceResponseDto<Comment>.Success(commentToFind);
        }

        public async Task<ServiceResponseDto<Comment>> Get(Guid commentId)
        {
            Comment commentToFind = await _commentRepository.GetAsync(commentId);
            if (commentToFind == null) {
                return ServiceResponseDto<Comment>.Failure("Cannot find comment of that id");
            }
            return ServiceResponseDto<Comment>.Success(commentToFind);
        }

        public async Task<ServiceResponseDto<IEnumerable<Comment>>> GetCommentsAboutProductAsync(Guid productBusinessKey)
        {
            var commentsAboutProduct = _commentRepository.GetAllAsQueryableAsync()
                                                         .Where(c => c.ProductBusinessKey.Equals(productBusinessKey))
                                                         .ToList();
            if (commentsAboutProduct == null) {
                return ServiceResponseDto<IEnumerable<Comment>>.Failure("The comment list about product is null, not even empty");
            }    
            return ServiceResponseDto<IEnumerable<Comment>>.Success(commentsAboutProduct);
        }

        public async Task<ServiceResponseDto<IEnumerable<Comment>>> GetCommentsOfUserAsync(Guid userId)
        {
            var commentsOfUser = _commentRepository.GetAllAsQueryableAsync()
                                                         .Where(c => c.UserId.Equals(userId))
                                                         .ToList();
            if (commentsOfUser == null) {
                return ServiceResponseDto<IEnumerable<Comment>>.Failure("The user comment list is null, not even empty");
            }
            return ServiceResponseDto<IEnumerable<Comment>>.Success(commentsOfUser);
        }

        public async Task<ServiceResponseDto<Comment>> Remove(Guid userId, Guid productBusinessKey)
        {
            var commentDeleted = await _commentRepository.RemoveAsync(userId, productBusinessKey);
            if (commentDeleted == null) {
                return ServiceResponseDto<Comment>.Failure("cannot delete comment, please check result of repo");
            }
            return ServiceResponseDto<Comment>.Success(commentDeleted);
        }

        public async Task<ServiceResponseDto<Comment>> Update(Guid userId, Guid productBusinessKey, string updatedCommentDetail)
        {
            var commentUpdated = await _commentRepository.UpdateAsync(userId, productBusinessKey, updatedCommentDetail);
            if (commentUpdated == null) {
                return ServiceResponseDto<Comment>.Failure("Cannot update comment because cannot findOne it, please check the result of repo");
            }

            return ServiceResponseDto<Comment>.Success(commentUpdated);
        }
    }
}
