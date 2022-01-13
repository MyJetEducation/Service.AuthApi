using System.ComponentModel.DataAnnotations;

namespace Service.AuthApi.Models
{
	public class LoginRequest
	{
		[Required]
		public string UserName { get; set; }

		[Required]
		public string Password { get; set; }
	}
}