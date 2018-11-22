

using PWANews.Models.DomainModels;

namespace PWANews.Interfaces
{
    public interface IUserAuthenticationService
    {
        bool AuthenticateUser(User user, string password);

        void SetOrRefreshAuthenticationToken(User user);

        string HashPassword(string password);

        bool ValidatePassword(string password, string hashedPassword);
    }
}
