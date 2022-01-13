namespace Service.AuthApi.Constants
{
	public class ResponseCode
	{
		public const int Ok = 0;

		public const int Fail = -1;

		public const int UserNotFound = -2;

		public const int NoRequestData = -3;

		public const int NoResponseData = -4;

		public const int UserAlreadyExists = -5;

		public const int NotValidPassword = -6;

		public const int NotValidEmail = -7;

		public const int NotValidFullName = -8;
	}
}