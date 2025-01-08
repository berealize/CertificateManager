using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace CertificateManager
{
    class Program3
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== DER 인증서와 개인 키 로드 프로그램 (.NET Framework 4.6.2) ===");

            // DER 인증서 경로 입력
            Console.Write("DER 인증서 파일 경로를 입력하세요: ");
            string certPath = Console.ReadLine();

            // PEM 개인 키 파일 경로 입력
            Console.Write("PEM 개인 키 파일 경로를 입력하세요: ");
            string keyPath = Console.ReadLine();

            if (!File.Exists(certPath) || !File.Exists(keyPath))
            {
                Console.WriteLine("파일 경로가 유효하지 않습니다.");
                return;
            }

            try
            {
                // DER 인증서 로드
                byte[] certBytes = File.ReadAllBytes(certPath);
                X509Certificate2 cert = new X509Certificate2(certBytes);

                // PEM 개인 키 로드
                AsymmetricKeyParameter privateKey = LoadPrivateKeyFromPem(keyPath);

                // 인증서와 개인 키 결합
                var certWithPrivateKey = AttachPrivateKey(cert, privateKey);

                // 인증서 정보 출력
                Console.WriteLine("\n=== 인증서와 개인 키가 성공적으로 로드되었습니다 ===");
                Console.WriteLine($"주체: {certWithPrivateKey.Subject}");
                Console.WriteLine($"발급자: {certWithPrivateKey.Issuer}");
                Console.WriteLine($"유효기간: {certWithPrivateKey.NotBefore} ~ {certWithPrivateKey.NotAfter}");
                Console.WriteLine($"고유 식별자(Thumbprint): {certWithPrivateKey.Thumbprint}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"오류가 발생했습니다: {ex.Message}");
            }
        }

        static AsymmetricKeyParameter LoadPrivateKeyFromPem(string keyPath)
        {
            using (var reader = File.OpenText(keyPath))
            {
                var pemReader = new PemReader(reader);
                var keyObject = pemReader.ReadObject();

                if (keyObject is AsymmetricCipherKeyPair keyPair)
                {
                    return keyPair.Private;
                }
                else if (keyObject is AsymmetricKeyParameter keyParam)
                {
                    return keyParam;
                }
                else
                {
                    throw new InvalidOperationException("유효한 개인 키를 찾을 수 없습니다.");
                }
            }
        }

        static X509Certificate2 AttachPrivateKey(X509Certificate2 cert, AsymmetricKeyParameter privateKey)
        {
            var rsaPrivateKey = DotNetUtilities.ToRSA(privateKey as Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters);
            var certWithKey = cert.Copy();
            certWithKey.PrivateKey = rsaPrivateKey;
            return certWithKey;
        }
    }

    //class Program3
    //{
    //    static void Main3(string[] args)
    //    {
    //        Console.WriteLine("=== DER 인증서와 개인 키 로드 프로그램 ===");

    //        // DER 인증서 경로 입력
    //        Console.Write("DER 인증서 파일 경로를 입력하세요: ");
    //        string certPath = Console.ReadLine();

    //        // 개인 키 파일 경로 입력
    //        Console.Write("PEM 개인 키 파일 경로를 입력하세요: ");
    //        string keyPath = Console.ReadLine();

    //        if (!File.Exists(certPath) || !File.Exists(keyPath))
    //        {
    //            Console.WriteLine("파일 경로가 유효하지 않습니다.");
    //            return;
    //        }

    //        try
    //        {
    //            // DER 인증서 로드
    //            byte[] certBytes = File.ReadAllBytes(certPath);
    //            X509Certificate2 cert = new X509Certificate2(certBytes);

    //            // PEM 개인 키 로드
    //            string pemKey = File.ReadAllText(keyPath);
    //            RSA privateKey = LoadPrivateKeyFromPem(pemKey);

    //            // 인증서와 개인 키 결합
    //            cert = cert.CopyWithPrivateKey(privateKey);

    //            // 인증서 정보 출력
    //            Console.WriteLine("\n=== 인증서와 개인 키가 성공적으로 로드되었습니다 ===");
    //            Console.WriteLine($"주체: {cert.Subject}");
    //            Console.WriteLine($"발급자: {cert.Issuer}");
    //            Console.WriteLine($"유효기간: {cert.NotBefore} ~ {cert.NotAfter}");
    //            Console.WriteLine($"고유 식별자(Thumbprint): {cert.Thumbprint}");
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"오류가 발생했습니다: {ex.Message}");
    //        }
    //    }

    //    static RSA LoadPrivateKeyFromPem(string pemKey)
    //    {
    //        // PEM 파일 파싱 (BEGIN/END 태그 제거)
    //        string base64Key = pemKey
    //            .Replace("-----BEGIN PRIVATE KEY-----", "")
    //            .Replace("-----END PRIVATE KEY-----", "")
    //            .Replace("-----BEGIN RSA PRIVATE KEY-----", "")
    //            .Replace("-----END RSA PRIVATE KEY-----", "")
    //            .Trim();

    //        byte[] keyBytes = Convert.FromBase64String(base64Key);

    //        // RSA 키 로드
    //        RSA privateKey = RSA.Create();
    //        privateKey.ImportPkcs8PrivateKey(keyBytes, out _);
    //        return privateKey;
    //    }
    //}
}