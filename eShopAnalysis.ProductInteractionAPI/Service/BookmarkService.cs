using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;
using eShopAnalysis.ProductInteractionAPI.Repository;
using System.Collections.Generic;

namespace eShopAnalysis.ProductInteractionAPI.Service
{
    public class BookmarkService: IBookmarkService
    {
        private readonly IBookmarkRepository _bookmarkRepository;
        public BookmarkService(IBookmarkRepository bookmarkRepository) { 
            _bookmarkRepository = bookmarkRepository;
        }

        public async Task<ServiceResponseDto<Bookmark>> Add(Guid userId, Guid productBusinessKey)
        {
            bool bookmarkExisted = await _bookmarkRepository.GetAsync(userId, productBusinessKey) != null;
            if (bookmarkExisted == true) {
                return ServiceResponseDto<Bookmark>.Failure("Cannot added bookmark because user already bookmark this product");
            }
            var bookmarkAdded = await _bookmarkRepository.AddAsync(userId, productBusinessKey);
            if (bookmarkAdded == null) {
                return ServiceResponseDto<Bookmark>.Failure("Cannot added bookmark because cannot find it");
            }

            return ServiceResponseDto<Bookmark>.Success(bookmarkAdded);
        }

        public async Task<ServiceResponseDto<Bookmark>> Get(Guid userId, Guid productBusinessKey)
        {
            Bookmark bookmarkToFind = await _bookmarkRepository.GetAsync(userId, productBusinessKey);
            if (bookmarkToFind == null) {
                return ServiceResponseDto<Bookmark>.Failure("Cannot find bookmark of user and product");
            }
            return ServiceResponseDto<Bookmark>.Success(bookmarkToFind);
        }

        public async Task<ServiceResponseDto<Bookmark>> Get(Guid bookmarkId)
        {
            Bookmark bookmarkToFind = await _bookmarkRepository.GetAsync(bookmarkId);
            if (bookmarkToFind == null) {
                return ServiceResponseDto<Bookmark>.Failure("Cannot find bookmark of that id");
            }
            return ServiceResponseDto<Bookmark>.Success(bookmarkToFind);
        }

        public async Task<ServiceResponseDto<IEnumerable<Bookmark>>> GetBookmarksOfUserAsync(Guid userId)
        {
            var bookmarksOfUser = _bookmarkRepository.GetAllAsQueryable()
                                                     .Where(b => b.UserId.Equals(userId))
                                                     .ToList();
            if (bookmarksOfUser == null)
            {
                return ServiceResponseDto<IEnumerable<Bookmark>>.Failure("bookmark list of user is null");
            }
            return ServiceResponseDto<IEnumerable<Bookmark>>.Success(bookmarksOfUser);
        }

        public async Task<ServiceResponseDto<Bookmark>> Remove(Guid userId, Guid productBusinessKey)
        {
            var bookmarkDeleted = await _bookmarkRepository.RemoveAsync(userId, productBusinessKey);
            if (bookmarkDeleted == null) {
                return ServiceResponseDto<Bookmark>.Failure("cannot delete bookmark, please check result of repo");
            }
            return ServiceResponseDto<Bookmark>.Success(bookmarkDeleted);
        }
    }
}
