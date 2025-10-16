using DataAccessLayer.Models;

namespace DataAccessLayer.IRepository
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken> GettokenAsync(string refreshtoken, CancellationToken cancellationToken);
        Task DeleteTokensForUserAsync(string UserId, CancellationToken cancellationToken);
    }
}
