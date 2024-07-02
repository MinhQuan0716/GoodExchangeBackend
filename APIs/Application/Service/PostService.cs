using Application.Common;
using Application.Criteria;
using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.Util;
using Application.ViewModel.CriteriaModel;
using Application.ViewModel.PostModel;
using Application.ViewModel.WishListModel;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AppConfiguration _appConfiguration;
        private readonly ICurrentTime _currentTime;
        private readonly IClaimService _claimService;
        private readonly IUploadFile _uploadFile;
        public PostService(IUnitOfWork unitOfWork, IMapper mapper, AppConfiguration appConfiguration, ICurrentTime currentTime
            , IClaimService claimService, IUploadFile uploadFile)
        {
            _uploadFile = uploadFile;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appConfiguration = appConfiguration;
            _currentTime = currentTime;
            _claimService = claimService;
        }

        public async Task<bool> AddPostToWishList(Guid postId)
        {
            var listPost = await _unitOfWork.PostRepository.GetAllPostsByCreatedByIdAsync(_claimService.GetCurrentUserId);
            var wishlist = await _unitOfWork.WishListRepository.FindWishListByPostId(postId);
            if (listPost.Where(x => x.Id == postId).Any())
            {
                throw new Exception("You cannot add your own post to favorite list");
            }
            if (wishlist.Where(x => x.UserId == _claimService.GetCurrentUserId).Any())
            {
                throw new Exception("The post already in favorite list");
            }
            var favoritePost = new WishList
            {
                UserId = _claimService.GetCurrentUserId,
                PostId = postId
            };
            await _unitOfWork.WishListRepository.AddAsync(favoritePost);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> BanPost(Guid postId)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(postId);
            if (post == null)
            {
                throw new Exception("Post not found");
            }
            _unitOfWork.PostRepository.SoftRemove(post);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> CheckIfPostInWishList(Guid postId)
        {
            var listFavoritePost = await SeeAllFavoritePost();
            bool isExsited = false;
            if(listFavoritePost.Where(x=>x.post.PostId== postId).Any()) 
            {
                isExsited = true;
            }
            return isExsited;
        }

        public async Task<bool> CreatePost(CreatePostModel postModel)
        {
            var imageUrl = await _uploadFile.UploadFileToFireBase(postModel.productModel.ProductImage, "Product");
            var newProduct = _mapper.Map<Product>(postModel.productModel);
            newProduct.ProductImageUrl = imageUrl;
            if (postModel.productModel.ConditionId == 2 || postModel.productModel.ProductPrice == null)
            {
                newProduct.ProductPrice = 0;
            }
            await _unitOfWork.ProductRepository.AddAsync(newProduct);
            await _unitOfWork.SaveChangeAsync();
            var createPost = new Post
            {
                PostTitle = postModel.PostTitle,
                PostContent = postModel.PostContent,
                Product = newProduct
            };
            await _unitOfWork.PostRepository.AddAsync(createPost);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> DeletePost(Guid PostId)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(PostId);
            if (post != null)
            {
                _unitOfWork.PostRepository.SoftRemove(post);
            }
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<Pagination<PostViewModel>> FilterPostByProductStatusAndPrice(string producttStatus,string exchangeCondition, int pageIndex, int pageSize)
        {
            var listPostModel = await _unitOfWork.PostRepository.GetAllPostWithDapper();
            ICriteria productStatusCriteria = new CriteriaProductStatus(producttStatus);
            ICriteria productPriceCriteria = new CriteriaExchangeCondition(exchangeCondition);
            ICriteria andCriteria = new AndCriteria(productStatusCriteria, productPriceCriteria);
            var filterPostList = andCriteria.MeetCriteria(listPostModel);
            var paginationFilterList=PaginationUtil<PostViewModel>.ToPagination(filterPostList, pageIndex, pageSize);
            return paginationFilterList;
        }

        public async Task<Pagination<PostViewModel>> GetAllPost(int pageIndex, int pageSize)
        {
            var listPostModel = await _unitOfWork.PostRepository.GetAllPostWithDapper();
            Pagination<PostViewModel> pagination = PaginationUtil<PostViewModel>.ToPagination(listPostModel, pageIndex, pageSize);
            return pagination;
        }

        public async Task<List<PostViewModel>> GetAllPostWithDapper()
        {
            var listPostWitDapper = await _unitOfWork.PostRepository.GetAllPostWithDapper();
            //var listPostViewModel = _mapper.Map<List<PostViewModel>>(listPostWitDapper);
            return listPostWitDapper;
        }

        public async Task<List<PostViewModel>> GetPostByCreatedById()
        {
            var id = _claimService.GetCurrentUserId;
            var posts = await _unitOfWork.PostRepository.GetAllPostsByCreatedByIdAsync(id);
            return _mapper.Map<List<PostViewModel>>(posts);
        }

        public async Task<PostDetailViewModel> GetPostDetailAsync(Guid postId)
        {
            var postDetail = await _unitOfWork.PostRepository.GetPostDetail(postId);
            return postDetail;
        }

        public async Task<PostDetailViewModel> GetPostDetailInUserCreatePostList(Guid postId)
        {
            var postDetail = await _unitOfWork.PostRepository.GetPostDetail(postId);
            return postDetail;
        }

        public async Task<List<PostViewModel>> GetPostSortByCreationDay()
        {
            var posts = await _unitOfWork.PostRepository.GetAllPostsWithDetailsSortByCreationDayAsync();
            return _mapper.Map<List<PostViewModel>>(posts);
        }

        public async Task<bool> RemovePostFromFavorite(Guid postId)
        {
            try
            {
                var foundList = await _unitOfWork.WishListRepository.FindWishListByPostId(postId);
                var wishList = foundList.Where(x => x.PostId == postId && x.UserId == _claimService.GetCurrentUserId).Single();
                if (wishList != null)
                {
                    _unitOfWork.WishListRepository.SoftRemove(wishList);
                }
            }
            catch
            {
                throw new Exception("You already remove this post");
            }
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<List<PostViewModel>> SearchPostByProductName(string productName)
        {
            var listSearchPost = await _unitOfWork.PostRepository.SearchPostByProductName(productName);
            return listSearchPost;
        }

        public async Task<List<WishListViewModel>> SeeAllFavoritePost()
        {
            var listFavoritePost = await _unitOfWork.WishListRepository.FindWishListByUserId(_claimService.GetCurrentUserId);
            return listFavoritePost;
        }

        public async Task<List<PostViewModel>> SortPostByCategory(int categoryId)
        {
            var sortPost = await _unitOfWork.PostRepository.SortPostByProductCategoryAsync(categoryId);
            return _mapper.Map<List<PostViewModel>>(sortPost);
        }

        public async Task<bool> UpdatePost(UpdatePostModel postModel)
        {
            var productId = await _unitOfWork.PostRepository.GetProductIdFromPostId(postModel.PostId);
            var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(productId);

            if (existingProduct == null)
            {
                // Handle the case where the product does not exist
                throw new Exception("Product not found");
            }

            // Map the updated product details
            _mapper.Map(postModel.productModel, existingProduct);
            if (postModel.productModel.ProductImage != null)
            {
                var imageUrl = await _uploadFile.UploadFileToFireBase(postModel.productModel.ProductImage, "Product");
                existingProduct.ProductImageUrl = imageUrl;
            }
            _unitOfWork.ProductRepository.Update(existingProduct);
            var oldPost = await _unitOfWork.PostRepository.GetByIdAsync(postModel.PostId);
            oldPost.PostTitle = postModel.PostTitle;
            oldPost.PostContent = postModel.PostContent;
            oldPost.Product = existingProduct;
            _unitOfWork.PostRepository.Update(oldPost);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }
    }
}
