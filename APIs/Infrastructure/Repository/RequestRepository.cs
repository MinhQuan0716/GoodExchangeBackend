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
    public class RequestRepository : GenericRepository<Request>,IRequestRepository
    {
        private readonly IDbConnection _connection;
        private readonly AppDbContext _dbContext;
        public RequestRepository(AppDbContext appDbContext, IClaimService claimService
            , ICurrentTime currentTime,IDbConnection connection) : base(appDbContext, claimService, currentTime)
        {
            _connection = connection;
            _dbContext = appDbContext;
        }

        public async Task<List<RequestViewModel>> GetAllRequestByCurrentUserId(Guid userId)
        {
            var sql = @"
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
            r.UserId = @UserId;
    ";

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

            return result.ToList();
        }
    }
}
