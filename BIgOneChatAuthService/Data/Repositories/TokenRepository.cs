using Data.Persistence;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace Data.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly DatabaseContext _databaseContext;

        public TokenRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<TokenModel> Register(TokenModel model)
        {
            await _databaseContext.Tokens.AddAsync(model);
            await _databaseContext.SaveChangesAsync();

            return model;
        }

        public async Task<TokenModel> GetByNickname(string nickname)
            => await _databaseContext.Tokens.FirstOrDefaultAsync(x => x.Nickname == nickname);


        public async Task<TokenModel> Update(TokenModel model)
        {
            _databaseContext.Tokens.Update(model);
            await _databaseContext.SaveChangesAsync();

            return model;
        }
    }
}
