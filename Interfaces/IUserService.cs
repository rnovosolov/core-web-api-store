using CoreWebAPIstore.Models;

namespace CoreWebAPIstore.Interfaces
{
    public interface IUserService
    {
        public User Get(UserLogin userLogin);
    }
}
