using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
namespace NZWalks.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly NZWalksDBContext nZWalksDBContext;
        public UserRepository(NZWalksDBContext nZWalksDBContext)
        {
            this.nZWalksDBContext = nZWalksDBContext;
        }
        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await nZWalksDBContext.Users
                                .FirstOrDefaultAsync(
                                    x => x.UserName.ToLower()==username.ToLower()
                                    && x.Password == password);
            if (user == null)
                return null;

            user.UserRoles = await nZWalksDBContext.Users_Roles.Where(x=>x.UserId==user.Id).ToListAsync();
            user.Password = null;
            return user;
        }
    }
}
