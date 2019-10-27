using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Testsa
{
    class Program
    {
        static string Remove(string r)
        {
            char[] chars = Path.GetInvalidFileNameChars();
            for (int i = 0; i < r.Length; i++)
            {
                for (int w = 0; w < chars.Length; w++)
                {
                    if (r[i] == chars[w])
                    {
                        r = r.Remove(i, 1);
                        break;
                    }
                }
            }
            return r;
        }

        static void Main(string[] args)
        {
           CoreLib core = new CoreLib();

            Console.Write("당신의 아이디를 입력 해주세요! : ");
  //          var id = Console.ReadLine();
            Console.Write("당신의 비밀번호를 입력 해주세요! : ");
    //        var pw = Console.ReadLine();

            if (core.Login("20183221", "20183221"))
            {
                Console.WriteLine("로그인 성공!");
            }
            else
            {
                Console.WriteLine("로그인 실패!");
            }

            Console.Write("저장할 경로! : ");
            var path = Console.ReadLine();

            // 파일화 작업

            Console.WriteLine("유저 정보 읽는중...");
            var user = core.GetUserInfo("20183221");
            Console.WriteLine("유저 정보 읽음!");

            Console.WriteLine($"ID : {user.Id}");
            Console.WriteLine($"Solve Problem : {user.SolvedProblem}");
            Console.WriteLine($"ConqeustRate : {user.ConqeustRate}");
            Console.WriteLine($"Attemp Solve : {user.AverageAttemptSolve}");
            Console.WriteLine($"Level : {user.Level}");

            Console.WriteLine();

            core.Submit("main(a,b){scanf(\"%d%d\",&a,&b);printf(\"%d\",a+b);}", 1000, Lang.Cpp);

            //foreach (var value in user.SolveList)
            //{
            //    Task t = new Task(() =>
            //    {
            //        Console.WriteLine($"{value.Id} 번 문제 상태창 검색중...");
            //        var status = core.SearchStatus(id, value.Id, Lang.All, Result.Accept);
            //        Console.WriteLine($"{value.Id} 번 문제 상태창 검색 완료");
            //        for (int i = 0; i < status.Count; i++)
            //        {
            //            Console.WriteLine($"{value.Id} 번 문제 코드 읽는 중...");
            //            var code = core.GetCode(status[i].CodeLink);
            //            Console.WriteLine($"{value.Id} 번 문제 코드 읽음");

            //            using (StreamWriter sw = new StreamWriter($"{path}\\{value.Id}_{Remove(value.Name)}_{status[i].Language}_{i + 1}.txt", false, Encoding.UTF8))
            //            {
            //                sw.Write(code);
            //            }
            //            Console.WriteLine($"{value.Id}_{Remove(value.Name)}_{status[i].Language}_{i + 1} 작성 저장 완료");
            //        }
            //    });
            //    t.Start();
            //}

            for (; ; );
        }
    }
}
