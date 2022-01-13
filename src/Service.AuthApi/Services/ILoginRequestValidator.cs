using Service.AuthApi.Models;

namespace Service.AuthApi.Services
{
	public interface ILoginRequestValidator
	{
		int? ValidateLoginRequest(LoginRequest request);

		int? ValidateRegisterRequest(RegisterRequest request);

		int? ValidatePassword(string value);
	}
}