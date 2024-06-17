using Application.InterfaceService;
using Application.Util;
using Application.ViewModel.PostModel;
using Application.ViewModel.ProductModel;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUploadFile _uploadFile;
        private readonly IClaimService _claimService;
        private readonly ICacheService _cacheService;
        public ProductService(IUnitOfWork unitOfWork,IMapper mapper,IUploadFile uploadFile,IClaimService claimService,ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uploadFile = uploadFile;
            _claimService = claimService;
            _cacheService = cacheService;
        }

       

        public async Task<bool> CreateProduct(CreatePostModel postModel)
        {
            var imageUrl = await _uploadFile.UploadFileToFireBase(postModel.productModel.ProductImage,"Product");
            var newProduct=_mapper.Map<Product>(postModel.productModel);
            newProduct.ProductImageUrl = imageUrl;
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

        public async Task<bool> DeleteProduct(Guid productId)
        {
            Product findProduct=await _unitOfWork.ProductRepository.GetByIdAsync(productId);
            if (findProduct!=null)
            {
                _unitOfWork.ProductRepository.SoftRemove(findProduct);
            }
            return await _unitOfWork.SaveChangeAsync()>0;
        }
        public async Task<List<Product>> GetAllProducts()
        {
            List<Product> products=await _unitOfWork.ProductRepository.GetAllAsync();
            return products;
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            _unitOfWork.ProductRepository.Update(product);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

    }
}
