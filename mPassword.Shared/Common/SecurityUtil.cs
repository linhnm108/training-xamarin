using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace mPassword.Shared
{
	public static class SecurityUtil
	{
		public static string SALT = "@B9-XamarinTraining";

		public static string HashSHA256(string password)
		{
			var hashedPwd = new StringBuilder();

			using (SHA256 hash = SHA256.Create())
			{
				Encoding enc = Encoding.UTF8;
				var result = hash.ComputeHash(enc.GetBytes(password));

				foreach (byte b in result)
				{
					hashedPwd.Append(b.ToString("x2"));
				}
			}
			return hashedPwd.ToString();
		}

		public static string Encrypt(string clearText)
		{
			byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
			var pdb = new PasswordDeriveBytes(SALT, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
			byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
			return Convert.ToBase64String(encryptedData);
		}

		public static string Decrypt(string cipherText)
		{
			byte[] cipherBytes = Convert.FromBase64String(cipherText);
			var pdb = new PasswordDeriveBytes(SALT, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
			byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
			return Encoding.Unicode.GetString(decryptedData);
		}

		static byte[] Encrypt(byte[] clearText, byte[] Key, byte[] IV)
		{
			var ms = new MemoryStream();
			Rijndael alg = Rijndael.Create();
			alg.Key = Key;
			alg.IV = IV;
			var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
			cs.Write(clearText, 0, clearText.Length);
			cs.Close();
			byte[] encryptedData = ms.ToArray();
			return encryptedData;
		}

		static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
		{
			var ms = new MemoryStream();
			Rijndael alg = Rijndael.Create();
			alg.Key = Key;
			alg.IV = IV;
			var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
			cs.Write(cipherData, 0, cipherData.Length);
			cs.Close();
			byte[] decryptedData = ms.ToArray();
			return decryptedData;
		}
	}
}