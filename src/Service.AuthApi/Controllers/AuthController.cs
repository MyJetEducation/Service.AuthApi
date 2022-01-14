using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Service.AuthApi.Models;
using Service.Authorization.Domain.Models;
using SimpleTrading.ClientApi.Utils;

namespace Service.AuthApi.Controllers
{
	[Authorize]
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
			TokenInfo tokenInfo = await _tokenService.GenerateTokensAsync(request.UserName, HttpContext.GetIp(), request.Password);

			return tokenInfo != null
				? DataResponse<TokenInfo>.Ok(tokenInfo)
				: Unauthorized();
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