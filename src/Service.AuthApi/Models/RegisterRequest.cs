using System.ComponentModel.DataAnnotations;

namespace Service.AuthApi.Models
{
	public class RegisterRequest: LoginRequest
	{
		[Required]
		public string FullName { get; set; }
	}
}