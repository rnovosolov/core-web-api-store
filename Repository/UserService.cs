using CoreWebAPIstore.Interfaces;
using CoreWebAPIstore.Models;

namespace CoreWebAPIstore.Repository
{
    public class UserService : IUserService

    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
        public User Get(UserLogin userLogin)
        {
            return _context.Users.FirstOrDefault(u => u.Username.Equals(userLogin.Username) && u.Password.Equals(userLogin.Password));
        }
    }
}
