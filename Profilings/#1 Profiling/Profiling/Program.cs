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

			var hash = new Rfc2898DeriveBytes("test", salt, 10000, HashAlgorithmName.SHA1).GetBytes(20);

			salt.CopyTo(hashBytes, 0);
			hash.CopyTo(hashBytes, 16);

			var passwordHash = Convert.ToBase64String(hashBytes);
		}
	}
}
