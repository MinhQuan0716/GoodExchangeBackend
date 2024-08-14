using Application.ViewModel.CategoryModel;
using Application.ViewModel.MessageModel;
using Application.ViewModel.PolicyModel;
using Application.ViewModel.PostModel;
using Application.ViewModel.ProductModel;
using Application.ViewModel.RatingModel;
using Application.ViewModel.RequestModel;
using Application.ViewModel.SubcriptionModel;
using Application.ViewModel.UserModel;
using Application.ViewModel.UserViewModel;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mappers
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateUserMap();
            CreateProductMap();
            CreatePostMap();
            PostMap();
            ProductMap();
            UpdatePostMap();
            UpdateProductMap();
            SubcriptionMap();
            RatingMap();
            MessageMap();
            OrderMap();
            CategoryMap();
            PolicyMap();
        }
        internal void CreateUserMap()
        {
            CreateMap<RegisterModel,User>().ReverseMap();
            CreateMap<UpdateUserProfileModel, User>()
                .ForMember(src => src.BirthDay, opt => opt.MapFrom(x => x.Birthday.ToDateTime(TimeOnly.MaxValue)))
                .ReverseMap()
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.BirthDay.Value)));
        }
        internal void CreateProductMap()
        {
            CreateMap<CreateProductModel,Product>()
                .ForMember(src=>src.CategoryId,opt=>opt.MapFrom(x=>x.CategoryId))
                .ForMember(src=>src.ConditionId,opt=>opt.MapFrom(x=>x.ConditionId))
                .ReverseMap();   
        }
        internal void UpdateProductMap()
        {
            CreateMap<UpdateProductModel, Product>()
                .ForMember(src => src.CategoryId, opt => opt.MapFrom(x => x.CategoryId))
                .ForMember(src => src.ConditionId, opt => opt.Ignore())
                .ReverseMap();
        }
        internal void CreatePostMap()
        {
            CreateMap<CreatePostModel, Post>()
                .ForMember(src=>src.PaymentType,opt=>opt.MapFrom(x=>x.PaymentType))
                .ReverseMap();
           
        }
        internal void PostMap()
        {
            CreateMap<PostViewModel, Post>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.PostId))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.PostId))
                .ReverseMap()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(x => x.Product))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(x => new DateOnly(x.CreationDate.GetValueOrDefault().Year, x.CreationDate.GetValueOrDefault().Month, x.CreationDate.GetValueOrDefault().Day)))
                .ForMember(dest=>dest.AuthorId,opt=>opt.MapFrom(x=>x.CreatedBy))
                .ForMember(dest=>dest.Location,opt=>opt.MapFrom(x=>x.Author.HomeAddress))
            ;
            CreateMap<PostViewModelForFeaturedImage, Post>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.PostId))
                .ReverseMap();
        }

        internal void ProductMap()
        {
            CreateMap<Product, ProductModel>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(x => x.Id))
                .ForMember(dest => dest.ConditionName, opt => opt.MapFrom(src => src.ConditionType.ConditionType))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ReverseMap();
        }
        internal void UpdatePostMap()
        {
            CreateMap<UpdatePostModel, Post>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.PostId))
                .ReverseMap();
        }
        internal void SubcriptionMap()
        {
            CreateMap<CreateSubcriptionModel, Subscription>()
                .ReverseMap();
            CreateMap<UpdateSubscriptionModel, Subscription>()
                .ReverseMap();
            CreateMap<SubscriptionDetailViewModel,Subscription>()
                .ReverseMap();
        }
        internal void RatingMap()
        {
            CreateMap<CreateRatingModel, Rating>()
                .ForMember(src=>src.RatedUserId,dest=>dest.MapFrom(rmodel=>rmodel.UserId))
                .ReverseMap();
        }
        internal void MessageMap()
        {
            CreateMap<CreateMessageModel, Message>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ChatRoomId, opt => opt.MapFrom(rmodel => rmodel.RoomId))
                .ReverseMap();
        }
        internal void OrderMap()
        {
            CreateMap<CreateOrderModel,Order>()
                .ForMember(dest=>dest.PostId,opt=>opt.MapFrom(model=>model.PostId))
                .ForMember(dest=>dest.UserId,opt=>opt.MapFrom(model=>model.AuthorId))
                .ReverseMap();
        }
        internal void CategoryMap()
        {
            CreateMap<CreateCategoryModel,Category>().ReverseMap();
            CreateMap<UpdateCategoryModel, Category>().ReverseMap();
        }
        internal void PolicyMap()
        {
            CreateMap<OrderCancelledTimeViewModel,Policy>()
                .ForMember(x=>x.Id,opt=>opt.MapFrom(src=>src.Id))
                .ForMember(x=>x.OrderCancelledAmount,opt=>opt.MapFrom(src=>src.OrderCancelledAmount))
                .ReverseMap();
            CreateMap<PostPriceViewModel,Policy>()
                .ForMember(x=>x.Id,opt=>opt.MapFrom(src=>src.Id))
                .ForMember(x=>x.PostPrice,opt=>opt.MapFrom(src=>src.PostPrice))
                .ReverseMap();
        }
    }
}
