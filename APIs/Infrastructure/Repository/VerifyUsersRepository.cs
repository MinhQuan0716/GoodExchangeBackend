using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.VerifyModel;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class VerifyUsersRepository : GenericRepository<VerifyUser>, IVerifyUsersRepository
    {
        private readonly AppDbContext _appDbContext;
        public VerifyUsersRepository(AppDbContext appDbContext, IClaimService claimService, ICurrentTime currentTime) : base(appDbContext, claimService, currentTime)
        {
            _appDbContext = appDbContext;
        }

        public async Task<VerifyUser> FindVerifyUserIdByUserId(Guid userId)
        {
            return await _appDbContext.VerifyUsers.Where(x => x.UserId == userId).Include(m => m.VerificationStatus).SingleOrDefaultAsync();
        }

        public async Task<List<VerifyViewModel>> GetAllVerifyUserAsync()
        {
            var listVerifyUser=await _appDbContext.VerifyUsers.Where(x=>x.IsDelete==false)
                                                              .Include(x=>x.User).ThenInclude(u=>u.Role).AsSplitQuery()
                                                              .Include(x=>x.VerificationStatus).AsSplitQuery()
                                                              .Select(x=>new VerifyViewModel
                                                              {
                                                                  Id= x.Id,
                                                                  Email=x.User.Email,
                                                                  ProfileImage=x.UserImage,
                                                                  RoleName=x.User.Role.RoleName,
                                                                  UserName=x.User.UserName,
                                                                  VerifyStatus=x.VerificationStatus.VerificationStatusName
                                                              }).ToListAsync();
            return listVerifyUser;
        }

        public async Task<VerifyViewModel> GetVerifyUserDetailAsync(Guid id)
        {
            var verfiyModel=await _appDbContext.VerifyUsers.Where(x=>x.IsDelete==false&&x.Id==id)
                                                            .Include(x => x.User).ThenInclude(u => u.Role).AsSplitQuery()
                                                              .Include(x => x.VerificationStatus).AsSplitQuery()
                                                              .Select(x => new VerifyViewModel
                                                              {
                                                                  Id = x.Id,
                                                                  Email = x.User.Email,
                                                                  ProfileImage = x.UserImage,
                                                                  RoleName = x.User.Role.RoleName,
                                                                  UserName = x.User.UserName,
                                                                  VerifyStatus = x.VerificationStatus.VerificationStatusName
                                                              }).SingleAsync();
            return verfiyModel;
        }
    }
}
