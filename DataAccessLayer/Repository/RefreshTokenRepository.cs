using DataAccessLayer.Data;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<RefreshToken> GettokenAsync(string refreshtoken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(e => e.Token == refreshtoken);
            return token;
        }
    }


}
