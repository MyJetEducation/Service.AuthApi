using Autofac;
using Microsoft.Extensions.Logging;
using Service.AuthApi.Services;
using Service.Registration.Client;
using Service.UserInfo.Crud.Client;
using Service.UserInfo.Crud.Grpc;

namespace Service.AuthApi.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterUserInfoCrudClient(Program.Settings.UserInfoCrudServiceUrl);
            builder.RegisterRegistrationClient(Program.Settings.RegistrationServiceUrl);

			builder.RegisterType<LoginRequestValidator>().AsImplementedInterfaces();

			builder.Register(context =>
				new TokenService(
					context.Resolve<IUserInfoService>(),
					Program.JwtSecret,
					Program.Settings.JwtTokenExpireMinutes,
					Program.Settings.RefreshTokenExpireMinutes,
					context.Resolve<ILogger<TokenService>>()))
				.As<ITokenService>()
				.SingleInstance();
		}
	}
}
