using System.Threading.Tasks;
using Service.AuthApi.Models;

namespace Service.AuthApi.Services
{
	public interface ITokenService
	{
		ValueTask<TokenInfo> GenerateTokensAsync(string userName, string ipAddress, string password = null);

		ValueTask<TokenInfo> RefreshTokensAsync(string currentRefreshToken, string ipAddress);
	}
}