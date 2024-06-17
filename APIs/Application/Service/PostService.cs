using Application.Common;
using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.Util;
using Application.ViewModel.PostModel;
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
            ,  IClaimService claimService, IUploadFile uploadFile)
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
            if (listPost.Where(x=>x.Id==postId).Any()) 
            {
                throw new Exception("You cannot add your own post to favorite list");
            }
            var favoritePost = new WishList
            {
                UserId=_claimService.GetCurrentUserId,
                PostId = postId 
            };
            await _unitOfWork.WishListRepository.AddAsync(favoritePost);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> BanPost(Guid postId)
        {
           var post= await _unitOfWork.PostRepository.GetByIdAsync(postId);
            if (post == null)
            {
                throw new Exception("Post not found");
            }
            _unitOfWork.PostRepository.SoftRemove(post);
            return await _unitOfWork.SaveChangeAsync()>0;
        }

        public async Task<bool> CreatePost(CreatePostModel postModel)
        {
            var imageUrl = await _uploadFile.UploadFileToFireBase(postModel.productModel.ProductImage, "Product");
            var newProduct = _mapper.Map<Product>(postModel.productModel);
            newProduct.ProductImageUrl = imageUrl;
            if (postModel.productModel.ConditionId == 2 || postModel.productModel.ProductPrice==null)
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

        public async Task<Pagination<PostModel>> GetAllPost(int pageIndex,int pageSize)
        {
            var posts = await _unitOfWork.PostRepository.GetAllPostsWithDetailsAsync();
           var  listPostModel =_mapper.Map<List<PostModel>>(posts);
            Pagination<PostModel> pagination = PaginationUtil<PostModel>.ToPagination(listPostModel, pageIndex, pageSize);
            return pagination;
        }

        public async Task<List<PostModel>> GetPostByCreatedById()
        {
            var id = _claimService.GetCurrentUserId;
            var posts = await _unitOfWork.PostRepository.GetAllPostsByCreatedByIdAsync(id);
            return _mapper.Map<List<PostModel>>(posts);
        }

        public async Task<PostDetailViewModel> GetPostDetailAsync(Guid postId)
        {
            var postDetail = await _unitOfWork.PostRepository.GetPostDetail(postId);
            return postDetail;
        }

        public async Task<List<PostModel>> GetPostSortByCreationDay()
        {
            var posts = await _unitOfWork.PostRepository.GetAllPostsWithDetailsSortByCreationDayAsync();
            return _mapper.Map<List<PostModel>>(posts);
        }

        public async Task<List<PostModel>> SortPostByCategory(int categoryId)
        {
            var sortPost=await _unitOfWork.PostRepository.SortPostByProductCategoryAsync(categoryId);
            return _mapper.Map<List<PostModel>>(sortPost);
        }

        public async Task<bool> UpdatePost(UpdatePostModel postModel)
        {
            var newProduct = _mapper.Map<Product>(postModel.productModel);
            if (postModel.productModel.ProductImage != null)
            {
                var imageUrl = await _uploadFile.UploadFileToFireBase(postModel.productModel.ProductImage,"Product");
                newProduct.ProductImageUrl = imageUrl;
            } else
            {
                var oldProduct = await _unitOfWork.ProductRepository.GetByIdAsync(postModel.productModel.ProductId);
                newProduct.ProductImageUrl = oldProduct.ProductImageUrl;
            }
            //_unitOfWork.ProductRepository.Update(newProduct);
            var oldPost = await _unitOfWork.PostRepository.GetByIdAsync(postModel.PostId);
            oldPost.PostTitle = postModel.PostTitle;
            oldPost.PostContent = postModel.PostContent;
            oldPost.Product = newProduct;
            _unitOfWork.PostRepository.Update(oldPost);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }
    }
}
