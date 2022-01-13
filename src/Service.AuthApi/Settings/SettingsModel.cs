using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.AuthApi.Settings
{
	public class SettingsModel
	{
		[YamlProperty("AuthApi.SeqServiceUrl")]
		public string SeqServiceUrl { get; set; }

		[YamlProperty("AuthApi.ZipkinUrl")]
		public string ZipkinUrl { get; set; }

		[YamlProperty("AuthApi.ElkLogs")]
		public LogElkSettings ElkLogs { get; set; }

		[YamlProperty("AuthApi.JwtTokenExpireMinutes")]
		public int JwtTokenExpireMinutes { get; set; }

		[YamlProperty("AuthApi.RefreshTokenExpireMinutes")]
		public int RefreshTokenExpireMinutes { get; set; }

		[YamlProperty("AuthApi.JwtAudience")]
		public string JwtAudience { get; set; }

		[YamlProperty("AuthApi.UserInfoCrudServiceUrl")]
		public string UserInfoCrudServiceUrl { get; set; }

		[YamlProperty("AuthApi.RegistrationServiceUrl")]
		public string RegistrationServiceUrl { get; set; }
	}
}