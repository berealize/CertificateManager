using System;
using System.Security.Cryptography.X509Certificates;

namespace CertificateManager
{
    class Program2
    {
        static void Main2(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== 공인인증서 관리 프로그램 ===");
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
            Console.WriteLine("\n=== 인증서 목록 ===");
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);

                if (store.Certificates.Count == 0)
                {
                    Console.WriteLine("인증서가 없습니다.");
                }
                else
                {
                    foreach (X509Certificate2 cert in store.Certificates)
                    {
                        if (cert.NotAfter >= DateTime.Now)
                            Console.WriteLine($"- 주체: {cert.Subject}, 만료일: {cert.NotAfter}");
                    }
                }

                store.Close();
            }
        }

        static void DeleteCertificate()
        {
            Console.Write("\n삭제할 인증서의 주체 이름 입력: ");
            string subjectName = Console.ReadLine();

            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);

                bool found = false;
                foreach (X509Certificate2 cert in store.Certificates)
                {
                    if (cert.NotAfter >= DateTime.Now)
                    {
                        //if (cert.Subject.Contains(subjectName, StringComparison.OrdinalIgnoreCase))
                        if (cert.Subject.ToLower().Contains(subjectName.ToLower()))
                        {
                            found = true;
                            store.Certificates.Remove(cert);
                            break;
                        }
                    }
                }

                if (!found)
                {
                    Console.WriteLine("일치하는 인증서를 찾을 수 없습니다.");
                }

                store.Close();
            }
        }

        static void ViewCertificateDetails()
        {
            Console.Write("\n세부 정보를 볼 인증서의 주체 이름 입력: ");
            string subjectName = Console.ReadLine();

            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);

                bool found = false;
                foreach (X509Certificate2 cert in store.Certificates)
                {
                    if (cert.NotAfter >= DateTime.Now)
                    {
                        //if (cert.Subject.Contains(subjectName, StringComparison.OrdinalIgnoreCase))
                        if (cert.Subject.ToLower().Contains(subjectName.ToLower()))
                        {
                            found = true;
                            Console.WriteLine("\n=== 인증서 세부 정보 ===");
                            Console.WriteLine($"주체: {cert.Subject}");
                            Console.WriteLine($"발급자: {cert.Issuer}");
                            Console.WriteLine($"유효기간: {cert.NotBefore} ~ {cert.NotAfter}");
                            Console.WriteLine($"고유 식별자(Thumbprint): {cert.Thumbprint}");
                            break;
                        }
                    }
                }

                if (!found)
                {
                    Console.WriteLine("일치하는 인증서를 찾을 수 없습니다.");
                }

                store.Close();
            }
        }
    }
}
