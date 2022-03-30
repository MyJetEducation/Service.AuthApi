using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Service.AuthApi.Constants;
using Service.AuthApi.Models;
using Service.Authorization.Client.Models;
using Service.Authorization.Client.Services;
using Service.Web;
using SimpleTrading.ClientApi.Utils;

namespace Service.AuthApi.Controllers
{
	[ApiController]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[OpenApiTag("Auth", Description = "user authorization")]
	[Route("/api/v1/auth")]
	public class AuthController : ControllerBase
	{
		private readonly ITokenService _tokenService;

		public AuthController(ITokenService tokenService) => _tokenService = tokenService;

		[AllowAnonymous]
		[HttpPost("login")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (TokenInfo), Description = "Ok")]
		[SwaggerResponse(HttpStatusCode.Unauthorized, null, Description = "Unauthorized")]
		public async ValueTask<IActionResult> LoginAsync([FromBody] LoginRequest request)
		{
			AuthTokenInfo tokenInfo = await _tokenService.GenerateTokensAsync(request.UserName, HttpContext.GetIp(), request.Password);
			if (tokenInfo == null)
				return StatusResponse.Error();

			return tokenInfo.IsValid()
				? DataResponse<TokenInfo>.Ok(tokenInfo)
				: StatusResponse.Error(
					tokenInfo.UserNotFound
						? ResponseCode.UserNotFound
						: tokenInfo.InvalidPassword
							? AuthResponseCodes.NotValidPassword
							: ResponseCode.Fail
					);
		}

		[AllowAnonymous]
		[HttpPost("refresh-token")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (TokenInfo), Description = "Ok")]
		[SwaggerResponse(HttpStatusCode.Forbidden, null, Description = "Forbidden")]
		public async ValueTask<IActionResult> RefreshTokenAsync([FromBody, Required] string refreshToken)
		{
			TokenInfo info = await _tokenService.RefreshTokensAsync(refreshToken, HttpContext.GetIp());

			return info == null
				? Forbid()
				: DataResponse<TokenInfo>.Ok(info);
		}
	}
}