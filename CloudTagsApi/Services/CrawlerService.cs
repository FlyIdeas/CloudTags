using CloudTagsApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CloudTagsApi.Services
{
    public class CrawlerService : ICrawlerService
    {
        private string getHTML(string Url)
        {
            string response = "";

            if (!Url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !Url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                Url = "http://" + Url;
            }

            HttpWebResponse objResponse;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(Url);
            objRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705)";
            objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr =
                new StreamReader(objResponse.GetResponseStream()))
            {
                response = sr.ReadToEnd();
                sr.Close();
            }
            return response;
        }       

        public async Task<Dictionary<string, int>> getWords(string Url, int TotalWords)
        {
            return await Task.Run(() =>
            {
                var Html = getHTML(Url);

                int start = Html.IndexOf("<body", StringComparison.CurrentCultureIgnoreCase);
                int end = Html.LastIndexOf("</body>", StringComparison.CurrentCultureIgnoreCase);
                var BodyHtml = Html.Substring(start, end - start + "</body>".Length);

                BodyHtml = Regex.Replace(BodyHtml, "<script.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                BodyHtml = Regex.Replace(BodyHtml, "<style.*?</style>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                BodyHtml = Regex.Replace(BodyHtml, "</?[a-z][a-z0-9]*[^<>]*>", "");
                BodyHtml = Regex.Replace(BodyHtml, "<!--(.|\\s)*?-->", "");
                BodyHtml = Regex.Replace(BodyHtml, "<!(.|\\s)*?>", "");
                BodyHtml = Regex.Replace(BodyHtml, "[\t\r\n]", " ");
                BodyHtml = Regex.Replace(BodyHtml, " {2,}", " ");
                BodyHtml = BodyHtml.Replace("&plus;", "", StringComparison.CurrentCultureIgnoreCase);
                BodyHtml = WebUtility.HtmlDecode(BodyHtml);
                BodyHtml = Regex.Replace(BodyHtml, @"[^a-zа-яA-ZА-Я0-9 -]", "").Trim().ToLower(); ;


                Dictionary<string, int> resultDictionary = new Dictionary<string, int>();
                foreach (var word in BodyHtml.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                {
                    if (resultDictionary.ContainsKey(word))
                    {
                        resultDictionary[word]++;
                    }
                    else
                    {
                        resultDictionary.Add(word, 1);
                    }
                }

                return resultDictionary.Select(item => new Word()
                {
                    Text = item.Key,
                    Weight = item.Value
                }).OrderByDescending(i => i.Weight).Take(TotalWords)
                .ToDictionary(pair => pair.Text, pair => pair.Weight); ;

            });
        }
    }
}


