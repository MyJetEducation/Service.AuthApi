using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Service.AuthApi.Constants;
using Service.AuthApi.Models;
using Service.AuthApi.Services;
using Service.Core.Domain.Extensions;
using Service.Core.Grpc.Models;
using Service.Registration.Grpc;
using Service.Registration.Grpc.Models;
using Service.UserInfo.Crud.Grpc;
using Service.UserInfo.Crud.Grpc.Models;
using SimpleTrading.ClientApi.Utils;

namespace Service.AuthApi.Controllers
{
	[ApiController]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[Route("/api/register/v1")]
	public class RegisterController : ControllerBase
	{
		private readonly ILoginRequestValidator _loginRequestValidator;
		private readonly IRegistrationService _registrationService;
		private readonly ITokenService _tokenService;
		private readonly IUserInfoService _userInfoService;

		public RegisterController(IUserInfoService userInfoService, ILoginRequestValidator loginRequestValidator, IRegistrationService registrationService, ITokenService tokenService)
		{
			_loginRequestValidator = loginRequestValidator;
			_registrationService = registrationService;
			_tokenService = tokenService;
			_userInfoService = userInfoService;
		}

		[HttpPost("create")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (StatusResponse), Description = "Ok")]
		public async ValueTask<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
		{
			int? validationResult = _loginRequestValidator.ValidateRegisterRequest(request);
			if (validationResult != null)
			{
				WaitFakeRequest();
				return StatusResponse.Error(validationResult.Value);
			}

			Guid? userId = await GetUserIdAsync(request.UserName);
			if (userId != null)
				return StatusResponse.Error(ResponseCode.UserAlreadyExists);

			CommonGrpcResponse response = await _registrationService.RegistrationAsync(new RegistrationGrpcRequest
			{
				UserName = request.UserName,
				Password = request.Password,
				FullName = request.FullName
			});

			return response?.IsSuccess == true ? StatusResponse.Ok() : StatusResponse.Error();
		}

		[HttpPost("confirm")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (DataResponse<TokenInfo>), Description = "Ok")]
		[SwaggerResponse(HttpStatusCode.Unauthorized, null, Description = "Unauthorized")]
		public async ValueTask<IActionResult> ConfirmRegisterAsync([FromBody, Required] string hash)
		{
			ConfirmRegistrationGrpcResponse response = await _registrationService.ConfirmRegistrationAsync(new ConfirmRegistrationGrpcRequest {Hash = hash});

			string userName = response?.Email;
			if (userName.IsNullOrEmpty())
				return StatusResponse.Error();

			TokenInfo tokenInfo = await _tokenService.GenerateTokensAsync(userName, HttpContext.GetIp());
			return tokenInfo != null
				? DataResponse<TokenInfo>.Ok(tokenInfo)
				: Unauthorized();
		}

		private async ValueTask<Guid?> GetUserIdAsync(string userName = null)
		{
			string identityName = userName ?? HttpContext.User.Identity?.Name;

			UserInfoResponse userInfoResponse = await _userInfoService.GetUserInfoByLoginAsync(new UserInfoAuthRequest
			{
				UserName = identityName
			});

			return userInfoResponse?.UserInfo?.UserId;
		}

		private static void WaitFakeRequest() => Thread.Sleep(200);
	}
}