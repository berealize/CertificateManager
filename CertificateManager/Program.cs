using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace CertificateManager
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
              -----BEGIN PRIVATE KEY-----
              -----END PRIVATE KEY-----
              -----BEGIN RSA PRIVATE KEY-----
              -----END RSA PRIVATE KEY-----
            */


            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== 공인인증서 관리 프로그램 (외부 드라이브) ===");
                Console.WriteLine("1. 인증서 목록 보기");
                Console.WriteLine("2. 인증서 세부 정보 보기");
                Console.WriteLine("3. 인증서 삭제");
                Console.WriteLine("4. 종료");
                Console.Write("선택: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ListCertificates();
                        break;
                    case "2":
                        ViewCertificateDetails();
                        break;
                    case "3":
                        DeleteCertificate();
                        break;
                    case "4":
                        Console.WriteLine("프로그램을 종료합니다.");
                        return;
                    default:
                        Console.WriteLine("잘못된 선택입니다.");
                        break;
                }

                Console.WriteLine("\n아무 키나 누르면 메인 메뉴로 돌아갑니다...");
                Console.ReadKey();
            }
        }

        static void ListCertificates()
        {
            Console.Write("\n확인할 드라이브나 폴더 경로 입력 (예: D:\\NPKI): ");
            string path = Console.ReadLine();

            if (string.IsNullOrEmpty(path)) path = @"D:\NPKI";

            if (!Directory.Exists(path))
            {
                Console.WriteLine("유효하지 않은 경로입니다.");
                return;
            }

            Console.WriteLine("\n=== 인증서 목록 ===");
            string[] dirs = Directory.GetDirectories(path, "CN=*", SearchOption.AllDirectories);

            int fileCnt = 0;

            foreach (string dir in dirs)
            {
                string[] files = Directory.GetFiles(dir, "*.der", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    fileCnt++;

                    Console.WriteLine($"{fileCnt}: {file}");
                }
            }

            if (fileCnt == 0)
            {
                Console.WriteLine("인증서 파일을 찾을 수 없습니다.");
            }
        }

        static void ViewCertificateDetails()
        {
            Console.Write("\n세부 정보를 확인할 인증서 파일 경로 입력: ");
            string filePath = Console.ReadLine();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("유효하지 않은 파일 경로입니다.");
                return;
            }

            try
            {
                Console.Write("인증서 비밀번호 입력: ");
                string password = Console.ReadLine();

                X509Certificate2 cert = new X509Certificate2(filePath, password);
                Console.WriteLine("\n=== 인증서 세부 정보 ===");
                Console.WriteLine($"주체: {cert.Subject}");
                Console.WriteLine($"발급자: {cert.Issuer}");
                Console.WriteLine($"유효기간: {cert.NotBefore} ~ {cert.NotAfter}");
                Console.WriteLine($"고유 식별자(Thumbprint): {cert.Thumbprint}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"오류 발생: {ex.Message}");
            }
        }

        static void DeleteCertificate()
        {
            Console.Write("\n삭제할 인증서 파일 경로 입력: ");
            string filePath = Console.ReadLine();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("유효하지 않은 파일 경로입니다.");
                return;
            }

            try
            {
                File.Delete(filePath);
                Console.WriteLine("인증서가 성공적으로 삭제되었습니다.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"삭제 중 오류 발생: {ex.Message}");
            }
        }
    }
}
