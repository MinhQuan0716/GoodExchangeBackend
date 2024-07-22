using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.PostModel;
using Application.ViewModel.RequestModel;
using Application.ViewModel.UserModel;
using Dapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly IDbConnection _connection;
        private readonly AppDbContext _dbContext;
        public OrderRepository(AppDbContext appDbContext, IClaimService claimService
            , ICurrentTime currentTime, IDbConnection connection) : base(appDbContext, claimService, currentTime)
        {
            _connection = connection;
            _dbContext = appDbContext;
        }

        public async Task<List<SentOrderViewModel>> GetAllRequestByCreatedByUserId(Guid userId)
        {
            var listRequest = await _dbContext.Orders.Where(x => x.IsDelete == false && x.CreatedBy == userId)
                                            .Include(x => x.User).ThenInclude(u => u.VerifyUser).AsSplitQuery()
                                            .Include(x => x.User).ThenInclude(u => u.Raters).AsSplitQuery()
                                            .Include(x => x.Post).AsSplitQuery()
                                            .Include(x => x.Status).AsSplitQuery()
                                            .Select(x => new SentOrderViewModel
                                            {
                                                OrderId = x.Id,
                                                OrderMessage = x.OrderMessage,
                                                OrderStatus=x.Status.StatusName,
                                                CreationDate = DateOnly.FromDateTime(x.CreationDate.Value),
                                                Post = new PostViewModelForRequest
                                                {
                                                    PostId = x.PostId,
                                                    PostContent = x.Post.PostContent,
                                                    PostTitle = x.Post.PostTitle
                                                },
                                                User = _dbContext.Users.Where(u => u.Id == x.UserId).AsSplitQuery().Select(u => new UserViewModelForRequest
                                                {
                                                    SenderId = x.CreatedBy.Value,
                                                    SenderEmail = u.Email,
                                                    SenderHomeAddress = u.HomeAddress,
                                                    SenderImageUrl = u.VerifyUser.UserImage,
                                                    SenderRating = (u.RatedUsers.Count() > 0
                                                                 ? u.RatedUsers.Sum(r => r.RatingPoint) / (u.RatedUsers.Count()) : 0),
                                                    SenderUsername = u.UserName
                                                }).Single()
                                            }).AsQueryable().AsNoTracking().ToListAsync();
            return listRequest;
        }

        public async Task<List<ReceiveOrderViewModel>> GetAllRequestByCurrentUserId(Guid userId)
        {
            /* var sql = @"
         SELECT 
             r.Id AS RequestId,
             r.RequestMessage,
             s.StatusName AS RequestStatus,
             u.Id AS SenderId,
             u.Email AS SenderEmail,
             u.UserName AS SenderUsername,
             p.Id AS PostId,
             p.PostContent,
             p.PostTitle
         FROM 
             Requests r
         INNER JOIN 
             Users u ON r.UserId = u.Id
         INNER JOIN 
             Posts p ON r.PostId = p.Id
         INNER JOIN 
             RequestStatuses s ON r.RequestStatusId = s.StatusId
         WHERE 
             r.UserId = @UserId;";

             var result = await _connection.QueryAsync<RequestViewModel, UserViewModelForRequest, PostViewModelForRequest, RequestViewModel>(
                 sql,
                 (request, user, post) =>
                 {
                     request.User = user;
                     request.Post = post;
                     return request;
                 },
                 new { UserId = userId },
                 splitOn: "SenderId, PostId"
             );

             return result.ToList();*/
            var listRequest = await _dbContext.Orders.Where(x => x.IsDelete == false && x.UserId == userId)
                                             .Include(x => x.User).ThenInclude(u => u.VerifyUser).AsSplitQuery()
                                             .Include(x=>x.User).ThenInclude(u=>u.Raters).AsSplitQuery()
                                             .Include(x => x.Post).AsSplitQuery()
                                             .Include(x => x.Status).AsSplitQuery()
                                             .Select(x => new ReceiveOrderViewModel
                                             {
                                                 OrderId = x.Id,
                                                 OrderMessage = x.OrderMessage,
                                                 OrderStatus=x.Status.StatusName,
                                                 CreationDate = DateOnly.FromDateTime(x.CreationDate.Value),
                                                 Post = new PostViewModelForRequest
                                                 {
                                                     PostId = x.PostId,
                                                     PostContent = x.Post.PostContent,
                                                     PostTitle = x.Post.PostTitle
                                                 },
                                                 User = _dbContext.Users.Where(u => u.Id == x.CreatedBy).AsSplitQuery().Select(u => new UserViewModelForRequest
                                                 {
                                                     SenderId = x.CreatedBy.Value,
                                                     SenderEmail = u.Email,
                                                     SenderHomeAddress = u.HomeAddress,
                                                     SenderImageUrl = u.VerifyUser.UserImage,
                                                     SenderRating = (u.RatedUsers.Count() > 0
                                                                  ? u.RatedUsers.Sum(r => r.RatingPoint) / (u.RatedUsers.Count()): 0),
                                                     SenderUsername=u.UserName
                                                 }).Single()
                                             }).AsQueryable().AsNoTracking().ToListAsync();
            return listRequest;
        }

        public async Task<List<Order>> GetRequestByPostId(Guid postId)
        {
            return await _dbContext.Orders.Where(x => x.PostId == postId).AsNoTracking().ToListAsync();
        }

        public async Task<List<Order>> GetRequestByUserIdAndPostId(Guid userId,Guid postId)
        {
            return await _dbContext.Orders.Where(x => x.UserId == userId&&x.PostId==postId).AsNoTracking().ToListAsync();
        }
    }
}
