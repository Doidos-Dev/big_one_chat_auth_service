using MongoDB.Bson;

namespace Domain.Models
{
    public class TokenModel
    {
        public ObjectId Id { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
