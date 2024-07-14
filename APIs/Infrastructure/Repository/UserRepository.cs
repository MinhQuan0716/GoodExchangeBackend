using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.UserModel;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IClaimService _claimService;
        public UserRepository(AppDbContext appDbContext, IClaimService claimService, ICurrentTime currentTime) : base(appDbContext, claimService, currentTime)
        {
            _dbContext = appDbContext;
            _claimService = claimService;
        }

        public async Task<User> FindUserByEmail(string email)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            User user= await _dbContext.Users.Include(x=>x.Role).FirstOrDefaultAsync(x=>x.Email==email);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            return user;
        }
        public async Task UpdateUserAsync(User user)
        {
            _dbSet.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<CurrentUserModel> GetCurrentLoginUserAsync(Guid userId)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _dbContext.Users.Where(x => x.Id == userId).Include(x=>x.VerifyUser).AsSplitQuery().Select(x => new CurrentUserModel
            {
                Userid=x.Id,
                Username=x.UserName,
                Email=x.Email,  
                Birthday=x.BirthDay.HasValue?DateOnly.FromDateTime(x.BirthDay.Value):null,
                Fullname = x.FirstName + " " + x.LastName,
                UserProfileImage=x.ProfileImage,
                Phonenumber=x.PhoneNumber,
                Rating=x.RatedUsers.Count()>0?
                x.RatedUsers.Sum(rate=>rate.RatingPoint)/x.RatedUsers.Count():0
            }).SingleOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<CurrentLoginUserForWebViewModel> GetCurrentLoginUserForWebAsync(Guid userId)
        {
          var currentUser= await _dbContext.Users.Where(x => x.Id == userId).Include(x=>x.Role).AsSplitQuery().Select(x => new CurrentLoginUserForWebViewModel
            {
               UserId=x.Id,
               Role=x.Role.RoleName,
               Email=x.Email
            }).AsSplitQuery().AsQueryable().AsNoTracking().SingleOrDefaultAsync();
            return currentUser;
        }

        public async Task<User> GetBannedUserById(Guid id)
        {
            var user = await _dbContext.Users.Where(x => x.IsDelete == true && x.Id == id).Include(x=>x.Role).SingleAsync();
            return user;
        }
        public async Task<UserDetailViewModel>GetUserDetail (Guid userId)
        {
            var user = await _dbContext.Users.Where(x => x.IsDelete == false && x.Id == userId)
                                            .Select(x => new UserDetailViewModel
                                            {
                                                Email=x.Email,  
                                                Username=x.UserName,
                                                Phonenumber=x.PhoneNumber,
                                                ProfileImage=x.ProfileImage,
                                                Birthday=x.BirthDay.HasValue?DateOnly.FromDateTime(x.BirthDay.Value):DateOnly.FromDateTime(DateTime.UtcNow),
                                                Fullname=x.FirstName+""+x.LastName
                                            }).SingleAsync();
            return user;
        }

        public async Task<List<UserViewModelForWeb>> GetAllUserForWeb()
        {
            var listUser = await _dbContext.Users.Select(x => new UserViewModelForWeb
            {
                UserId=x.Id,
                Username=x.UserName,
                Email=x.Email,
                Fullname=x.FirstName+""+x.LastName,
                Status=x.IsDelete.Value?"Ban":"Not ban"
            }).ToListAsync();
            return listUser;
        }
    }
}
