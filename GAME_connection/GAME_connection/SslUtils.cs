using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;
using System.IO;

namespace GAME_connection {
	internal static class SslUtils {

		internal static bool IsAdministrator() {
			var identity = WindowsIdentity.GetCurrent();
			var principal = new WindowsPrincipal(identity);
			return principal.IsInRole(WindowsBuiltInRole.Administrator);
		}

		internal static X509Certificate LoadServerCertificate(string certificatePath, bool printInfo, TcpConnection.Logger logger) {
			X509Certificate serverCertificateObject = new X509Certificate();
			serverCertificateObject.Import(File.ReadAllBytes(certificatePath));
			if (printInfo) {
				logger("server public key string for certificate named: " + certificatePath);
				logger(serverCertificateObject.GetPublicKeyString());
			}
			return serverCertificateObject;
		}

		/// <summary>
		/// compares received certificate public key with known public key of server. Does not require importing servers certificate to trusted root certificates via MMC
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="certificate"></param>
		/// <param name="chain"></param>
		/// <param name="sslPolicyErrors"></param>
		/// <returns></returns>
		internal static bool ValidateServerCertificateNoImport(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
			string receivedCertificatePublicKeyString = certificate.GetPublicKeyString();
			if (receivedCertificatePublicKeyString == PublicKeys.USED_PUBLIC_KEY) return true;
			else {
				Console.WriteLine("Received public key does not match the known one");
				return false;
			}
		}

		/// <summary>
		/// Checks validity of received server certificate, reqires the client to import server certificate to trusted root certificates via MMC
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="certificate"></param>
		/// <param name="chain"></param>
		/// <param name="sslPolicyErrors"></param>
		/// <returns></returns>
		internal static bool ValidateServerCertificateWithTrustedRootCertificateCheck(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
			if (sslPolicyErrors == SslPolicyErrors.None) return true;
			Console.WriteLine("Certificate error: " + sslPolicyErrors);
			return false;
		}

	}

}
