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
            return await _dbContext.Users.Where(x => x.Id == userId).Select(x => new CurrentUserModel
            {
                Userid=x.Id,
                Username=x.UserName,
                Email=x.Email,  
                Birthday=x.BirthDay.HasValue?DateOnly.FromDateTime(x.BirthDay.Value):null,
                Fullname = x.FirstName + " " + x.LastName,
                Phonenumber=x.PhoneNumber,
                Rating=x.RatedUsers.Count()>0?
                x.RatedUsers.Sum(rate=>rate.RatingPoint)/x.RatedUsers.Count():0
            }).SingleOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
