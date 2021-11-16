using System;
using System.Security.Cryptography;

namespace Profiling
{
	class Program
	{
		static void Main()
		{
			var salt = new byte[16];
			var hashBytes = new byte[36];

			var hash = new Rfc2898DeriveBytes("test", salt, 10000).GetBytes(20);

			Array.Copy(salt, 0, hashBytes, 0, 16);
			Array.Copy(hash, 0, hashBytes, 16, 20);

			var passwordHash = Convert.ToBase64String(hashBytes);
		}
	}
}
