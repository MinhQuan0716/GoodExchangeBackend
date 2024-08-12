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
using Hangfire;
using Microsoft.IdentityModel.Tokens;
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
        private readonly string _cacheKey = "POST_PRICE";
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AppConfiguration _appConfiguration;
        private readonly ICurrentTime _currentTime;
        private readonly IClaimService _claimService;
        private readonly IUploadFile _uploadFile;
        private readonly ICacheService _setting;
        private readonly IBackgroundJobClient _backgroundJobClient;
        public PostService(IUnitOfWork unitOfWork, IMapper mapper, AppConfiguration appConfiguration, ICurrentTime currentTime
            , IClaimService claimService, IUploadFile uploadFile, IBackgroundJobClient backgroundJobClient,ICacheService setting)
        {
            _backgroundJobClient = backgroundJobClient;
            _uploadFile = uploadFile;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appConfiguration = appConfiguration;
            _currentTime = currentTime;
            _claimService = claimService;
            _setting=setting;
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
            var isSave = false;
            float amount = _setting.GetData<float>(_cacheKey);
            var verifyStatus = await _unitOfWork.VerifyUsersRepository.GetVerifyUserDetailByUserIdAsync(_claimService.GetCurrentUserId);
            if(verifyStatus.VerifyStatus=="Pending" || verifyStatus.VerifyStatus == "Denied")
            {
                throw new Exception("You must be verified to be able to do this action");
            }
            if (postModel.productModel.ConditionId != 3)
            {
                if (postModel.PaymentType == "Subscription")
                {
                    var listSubscription = await _unitOfWork.SubscriptionHistoryRepository.GetUserPurchaseSubscription(_claimService.GetCurrentUserId);
                    if (listSubscription != null)
                    {
                        if (listSubscription.Count() == 0)
                        {
                            throw new Exception("You must subscribe to  create post");
                        }
                    }
                    if (listSubscription.Any(ls => ls.Status == "Available"))
                    {
                        var isPriority = false;
                        var imageUrl = await _uploadFile.UploadFileToFireBase(postModel.productModel.ProductImage, "Product");
                        var newProduct = _mapper.Map<Product>(postModel.productModel);
                        var subscriptionId = listSubscription.Where(ls => ls.Status == "Available").Select(sh => sh.SubscriptionId).FirstOrDefault();
                        if (subscriptionId != null)
                        {
                            var subscription = await _unitOfWork.SubcriptionRepository.GetByIdAsync(subscriptionId);
                            if (subscription != null)
                            {
                                if (subscription.Description == "Priority")
                                {
                                    isPriority = true;
                                }
                            }
                        }
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
                            Product = newProduct,
                            UserId = _claimService.GetCurrentUserId,
                            IsPriority = isPriority
                        };
                        await _unitOfWork.PostRepository.AddAsync(createPost);
                    }
                    else
                    {
                        throw new Exception("Your subscription is expired");
                    }
                }
                if (postModel.PaymentType == "Wallet")
                {
                    var userWallet = await _unitOfWork.WalletRepository.GetUserWalletByUserId(_claimService.GetCurrentUserId);
                    if (userWallet.UserBalance < amount)
                    {
                        throw new Exception("Your user balance is not enough to purchase this post");
                    }
                    userWallet.UserBalance -= amount;
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
                        Product = newProduct,
                        UserId = _claimService.GetCurrentUserId,
                        IsPriority = false,
                    };
                    await _unitOfWork.PostRepository.AddAsync(createPost);
                    _unitOfWork.WalletRepository.Update(userWallet);
                    await _unitOfWork.SaveChangeAsync();
                    isSave = true;
                    _backgroundJobClient.Schedule(() => DeletePost(createPost.Id), TimeSpan.FromHours(24));
                }
            }
            else if (postModel.productModel.ConditionId == 3)
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
                    Product = newProduct,
                    UserId = _claimService.GetCurrentUserId,
                    IsPriority = true
                };
                await _unitOfWork.PostRepository.AddAsync(createPost);
            }
            if (isSave)
            {
                return isSave;
            }
            else
            {
                return await _unitOfWork.SaveChangeAsync() > 0;
            }

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

        public async Task<List<PostViewModel>> SearchPostByPostTitleAndFilterPostByProductStatusAndPrice(string postTitle,string productStatus,string exchangeCondition)
        {
            var allPosts = await _unitOfWork.PostRepository.GetAllPost(_claimService.GetCurrentUserId);

            if (string.IsNullOrEmpty(postTitle) && string.IsNullOrEmpty(productStatus) && string.IsNullOrEmpty(exchangeCondition))
            {
                // Return all posts if no criteria provided
                return allPosts;
            }

            var filteredPosts = allPosts.AsEnumerable();

            if (!string.IsNullOrEmpty(postTitle))
            {
                filteredPosts = filteredPosts.Where(x => ContainInOrder.ContainsInOrder(x.PostTitle.ToLower(), postTitle.ToLower()));
            }

            if (!string.IsNullOrEmpty(productStatus))
            {
                filteredPosts = filteredPosts.Where(x => x.Product.ProductStatus == productStatus);
            }

            if (!string.IsNullOrEmpty(exchangeCondition))
            {
                filteredPosts = filteredPosts.Where(x => x.Product.ConditionName == exchangeCondition);
            }

            return filteredPosts.ToList();
        }

        public async Task<List<PostViewModel>> GetAllPost()
        {
            var listPostModel = await _unitOfWork.PostRepository.GetAllPost(_claimService.GetCurrentUserId);
            return listPostModel;
        }

        public async Task<List<PostViewModelForWeb>> GetAllPostForWeb()
        {
          var listPost= await _unitOfWork.PostRepository.GetAllPostForWebAsync();
          return listPost;
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
            var posts = await _unitOfWork.PostRepository.GetAllPostsWithDetailsSortByCreationDayAsync(_claimService.GetCurrentUserId);
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

        public async Task<List<PostViewModel>> SearchPostCreatedByCurrentUserByPostTitle(string postTitle)
        {
            var userCreatedListPost = await _unitOfWork.PostRepository.GetAllPostsByCreatedByIdAsync(_claimService.GetCurrentUserId);
            var searchListPost= userCreatedListPost.Where(x => ContainInOrder.ContainsInOrder(x.PostTitle.ToLower(), postTitle.ToLower())).ToList();
            var searchPostModel=_mapper.Map<List<PostViewModel>>(searchListPost);
            return searchPostModel;
        }

        public async Task<List<WishListViewModel>> SeeAllFavoritePost()
        {
            var listFavoritePost = await _unitOfWork.WishListRepository.FindWishListByUserId(_claimService.GetCurrentUserId);
            return listFavoritePost;
        }

        public async Task<List<PostViewModel>> SortPostByCategory(int categoryId, List<PostViewModel>? dataPost)
        {
            if (dataPost != null)
            {
                var sortPostList=dataPost.Where(x=>x.Product.CategoryId== categoryId).ToList();
                return sortPostList;
            } else
            {
                var sortPost = await _unitOfWork.PostRepository.SortPostByProductCategoryAsync(categoryId);
                var sortPostViewModel = _mapper.Map<List<PostViewModel>>(sortPost);
                return sortPostViewModel;
            }
        }

        public async Task<bool> UnbanPost(Guid postId)
        {
            var post = await _unitOfWork.PostRepository.GetBannedPostById(postId);
            if (post == null)
            {
                throw new Exception("The post is active");
            }
            post.IsDelete = false;
            _unitOfWork.PostRepository.Update(post);
            return await _unitOfWork.SaveChangeAsync() > 0;
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

        public async Task<bool> RemovePostWhenSubscriptionExpire()
        {
            bool isDeleted = false;
            var listUser = await _unitOfWork.UserRepository.GetAllMember();
            foreach(var user in listUser)
            {
                var listUserPurchaseSubscription=await _unitOfWork.SubscriptionHistoryRepository.GetUserPurchaseSubscription(user.Id);
               /* var listUserExpireSubscription = await _unitOfWork.SubscriptionHistoryRepository.GetUserExpireSubscription(user.Id);*/
                if (listUserPurchaseSubscription != null)
                {
                    if (listUserPurchaseSubscription.Count() > 0)
                    {
                        if (listUserPurchaseSubscription.Where(x => x.Status == "Expried").Count() == listUserPurchaseSubscription.Count())
                        {
                            var listPostCreatedByUser = await _unitOfWork.PostRepository.GetAllPostsByCreatedByIdAsync(user.Id);
                            _unitOfWork.PostRepository.SoftRemoveRange(listPostCreatedByUser);
                            isDeleted = await _unitOfWork.SaveChangeAsync() > 0;
                        }
                    }
                    
                } else
                {
                    var listPostCreatedByUser = await _unitOfWork.PostRepository.GetAllPostsByCreatedByIdAsync(user.Id);
                      if(listPostCreatedByUser != null)
                    {
                       foreach(var post in listPostCreatedByUser)
                        {
                            if (post.Product.ConditionId != 3)
                            {
                                _unitOfWork.PostRepository.SoftRemove(post);
                                isDeleted = await _unitOfWork.SaveChangeAsync() > 0;
                            }
                        }
                    }
                }
            }
            return isDeleted;
        }
    }
}
