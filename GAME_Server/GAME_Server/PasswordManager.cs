using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace GAME_Server {
	internal class PasswordManager {
		private static readonly int SALT_SIZE = 16;		//number of bytes in salt
		private static readonly int HASH_SIZE = 32;		//number of bytes in hash
		private static readonly int HASH_ITER = 10000;

		internal static string GeneratePasswordHash(string plainTextPassword) {
			byte[] salt = new byte[SALT_SIZE];
			byte[] hash = new byte[HASH_SIZE];

			//generate new salt value
			using (RNGCryptoServiceProvider cryptoRng = new RNGCryptoServiceProvider()) {
				cryptoRng.GetBytes(salt);
			}

			using (Rfc2898DeriveBytes passwdHash = new Rfc2898DeriveBytes(plainTextPassword, salt, HASH_ITER)) {
				hash = passwdHash.GetBytes(HASH_SIZE);
			}

			byte[] saltHash = CombineHashAndSalt(salt, hash);

			return Convert.ToBase64String(saltHash);
		}

		private static byte[] CombineHashAndSalt(byte[] salt, byte[] hash) {
			byte[] array = new byte[SALT_SIZE + HASH_SIZE];
			Array.Copy(salt, 0, array, 0, SALT_SIZE);
			Array.Copy(hash, 0, array, SALT_SIZE, HASH_SIZE);
			return array;
		}
		
		internal static bool VerifyPassword(string savedHashedPassword, string checkedPlainTextPassword) {
			byte[] storedPasswdHash = Convert.FromBase64String(savedHashedPassword);

			//get salt from stored password
			byte[] salt = new byte[SALT_SIZE];
			Array.Copy(storedPasswdHash, 0, salt, 0, SALT_SIZE);

			//hash checked plain text password and compare its hash with stored hash value
			byte[] checkedPasswdHash;
			using (Rfc2898DeriveBytes passwdHash = new Rfc2898DeriveBytes(checkedPlainTextPassword, salt, HASH_ITER)) {
				checkedPasswdHash = passwdHash.GetBytes(HASH_SIZE);
			}
			for (int i = 0; i < HASH_SIZE; i++) {
				if (checkedPasswdHash[i] != storedPasswdHash[i + SALT_SIZE]) return false;
			}
			return true;
		}

	}

}
