using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Testsa
{
    public class CoreLib
    {
        CookieContainer Session = null;

        public bool Login(string id, string pw)
        {
            HttpWebRequest hwr = (HttpWebRequest)WebRequest.CreateHttp("https://lavida.us/login.php");
            hwr.Method = "POST";
            hwr.ContentType = "application/x-www-form-urlencoded";
            hwr.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            hwr.Host = "lavida.us";
            hwr.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";
            hwr.Referer = "https://lavida.us/status.php";
            hwr.CookieContainer = new CookieContainer();

            byte[] bytes = Encoding.ASCII.GetBytes($"user_id={id}&password={pw}");

            hwr.ContentLength = bytes.Length; // 바이트수 지정

            using (Stream reqStream = hwr.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);
            }

            // Response 처리
            string responseText = string.Empty;
            using (WebResponse resp = hwr.GetResponse())
            {
                
                Stream respStream = resp.GetResponseStream();
                using (StreamReader sr = new StreamReader(respStream))
                {
                    responseText = sr.ReadToEnd();
                    if (responseText.Contains("UserName or Password Wrong!"))
                    {
                        return false;
                    }
                }
            }
            Session = hwr.CookieContainer;
            return true;
        }

        public UserInfo GetUserInfo(string id)
        {
            UserInfo result = new UserInfo();

            WebClient wc = new WebClient
            {
                Encoding = Encoding.UTF8
            };

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(wc.DownloadString($"https://lavida.us/userinfo.php?user={id}"));

            var p = document.DocumentNode.SelectNodes("//body//div[@class='container']//div/div[@class='row']");
            var stat = p[1].SelectNodes("./div[@class='col-md-4']//table/tbody/tr");

            result.Id = id;
            result.SolvedProblem = stat[0].SelectNodes("./td")[1].InnerText.Trim();
            result.ConqeustRate = stat[1].SelectNodes("./td")[1].InnerText.Trim();
            result.AverageAttemptSolve = stat[2].SelectNodes("./td")[1].InnerText.Trim();
            result.Level = stat[3].SelectNodes("./td")[1].InnerText.Trim();

            var problem = p[1].SelectNodes("./div[@class='col-md-8']/table")[1];
            var list = problem.SelectNodes("./tbody/tr/td/a");

            foreach (var value in list)
            {
                var data = new Problem();

                var v = value.InnerText.Split(' ')[0];

                data.Id = int.Parse(v);
                data.Name = value.InnerText.Remove(0, 5).Trim();

                result.SolveList.Add(data);
            }

            return result;
        }

        string GetPage(string page)
        {
            HttpWebRequest hwr = (HttpWebRequest)WebRequest.CreateHttp(page);

            hwr.Method = "GET";
            hwr.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";
            hwr.ContentType = "text/html; charset=utf-8";
            hwr.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            hwr.CookieContainer = new CookieContainer();
            hwr.CookieContainer = Session;
            var s = hwr.GetResponse().GetResponseStream();
            var stream = new StreamReader(s);

            return stream.ReadToEnd();
        }

        string PostPage(string page, string value)
        {
            HttpWebRequest hwr = (HttpWebRequest)WebRequest.CreateHttp(page);

            hwr.Method = "POST";
            hwr.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";
            hwr.ContentType = "text/html; charset=utf-8";
            hwr.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            hwr.CookieContainer = new CookieContainer();
            hwr.CookieContainer = Session;
            hwr.Referer = "https://lavida.us/submitpage.php?id=1002";

            using (var stream = hwr.GetRequestStream())
            {
                using (var sw = new StreamWriter(stream))
                {
                    sw.Write(value);
                }
            }

            using (var read = hwr.GetResponse().GetResponseStream())
            {
                using (var sr = new StreamReader(read))
                {
                    var p = sr.ReadToEnd();
                    return p;
                }
            }

            return null;
        }
        
        public List<StatusList> SearchStatus(string id, int problemId, Lang l = Lang.All, Result r = Result.All)
        {
            List<StatusList> result = new List<StatusList>();

            var code = GetPage($"https://lavida.us/status.php?user_id={id}&problem_id={problemId}&language={(int)l}&jresult={(int)r}");

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(code);

            var list = document.DocumentNode.SelectNodes("//body//div[@class='container']");

            list = list[1].SelectNodes("./div/table/tr");
            foreach (var value in list)
            {
                StatusList tmp = new StatusList();

                var data = value.SelectNodes("./td");

                tmp.RunId = int.Parse(data[0].InnerText);
                tmp.Name = data[1].InnerText;
                tmp.ProblemId = int.Parse(data[2].InnerText);
                tmp.Reuslt = data[3].InnerText;
                tmp.Memory = data[4].InnerText;
                tmp.Time = data[5].InnerText;
                tmp.Language = data[6].InnerText;
                tmp.CodeLength = data[7].InnerText;
                tmp.CodeLink = $"https://lavida.us/{data[6].FirstChild.Attributes["href"].Value}";
                tmp.SubmitTime = data[8].InnerText;

                result.Add(tmp);
            }

            return result;
        }

        public string GetCode(string link)
        {
            var code = GetPage(link);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(code);

            var p = document.DocumentNode.SelectSingleNode("//body//div[@class='container']//div//textarea");


            return HttpUtility.HtmlDecode(p.InnerText);
        }


        public bool Submit(string value, int problemId, Lang l = Lang.C)
        {
            PostPage("https://lavida.us/submit.php", $"id=1002&language=0&source=main%28a%2Cb%29%7Bscanf%28%22%25d%25d%22%2C%26a%2C%26b%29%3Bprintf%28%22%25d%22%2Ca%2Bb%29%3B%7D");

            // https://lavida.us/submit.php
            // 302 POST

            // id=1000&language=1&source=main%28a%2Cb%29%7Bscanf%28%22%25d%25d%22%2C%26a%2C%26b%29%3Bprintf%28%22%25d%22%2Ca%2Bb%29%3B%7D




            return true;
        }
    }
}
