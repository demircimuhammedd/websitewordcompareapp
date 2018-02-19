using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using websitewordcompareapp.Models;

namespace websitewordcompareapp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AnalysiseAsync([FromBody]Analysis model)
        {
            if (ModelState.IsValid)
            {
                var listWebPage1 = FillListWebsite(CallWebsiteHtml("https://www.crimsonhexagon.com/"));
                var listWebPage2 = FillListWebsite(CallWebsiteHtml("https://www.brandwatch.com/"));

                List<AnalysisResult> listAnalyzeResult = new List<AnalysisResult>();

                foreach (var webPage1 in listWebPage1)
                {
                    var webPage2 = listWebPage2.SingleOrDefault(q => q.Key.Equals(webPage1.Key));
                    AnalysisResult analyzeResult = new AnalysisResult
                    {
                        Page1Word = webPage1.Key,
                        Page1Count = webPage1.Value.ToString(),
                        Page2Word = webPage2.Value > 0 ? webPage2.Key : "-",
                        Page2Count = webPage2.Value > 0 ? webPage2.Value.ToString() : string.Empty
                    };

                    listAnalyzeResult.Add(analyzeResult);
                }

                return Json(listAnalyzeResult);
            }
            return Json("Error!");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private static string CallWebsiteHtml(string urlAddress)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();
                response.Close();
                readStream.Close();

                return data;
            }
            return string.Empty;
        }
        private static IOrderedEnumerable<KeyValuePair<string, int>> FillListWebsite(string data)
        {
            string[] marks = new string[] { "a", "about", "above", "above", "across", "after", "afterwards", "again", "against", "all", "almost", "alone", "along", "already", "also", "although", "always", "am", "among", "amongst", "amoungst", "amount", "an", "and", "another", "any", "anyhow", "anyone", "anything", "anyway", "anywhere", "are", "around", "as", "at", "back", "be", "became", "because", "become", "becomes", "becoming", "been", "before", "beforehand", "behind", "being", "below", "beside", "besides", "between", "beyond", "bill", "both", "bottom", "but", "by", "call", "can", "cannot", "cant", "co", "con", "could", "couldnt", "cry", "de", "describe", "detail", "do", "done", "down", "due", "during", "each", "eg", "eight", "either", "eleven", "else", "elsewhere", "empty", "enough", "etc", "even", "ever", "every", "everyone", "everything", "everywhere", "except", "few", "fifteen", "fify", "fill", "find", "fire", "first", "five", "for", "former", "formerly", "forty", "found", "four", "from", "front", "full", "further", "get", "give", "go", "had", "has", "hasnt", "have", "he", "hence", "her", "here", "hereafter", "hereby", "herein", "hereupon", "hers", "herself", "him", "himself", "his", "how", "however", "hundred", "ie", "if", "in", "inc", "indeed", "interest", "into", "is", "it", "its", "itself", "keep", "last", "latter", "latterly", "least", "less", "ltd", "made", "many", "may", "me", "meanwhile", "might", "mill", "mine", "more", "moreover", "most", "mostly", "move", "much", "must", "my", "myself", "name", "namely", "neither", "never", "nevertheless", "next", "nine", "no", "nobody", "none", "noone", "nor", "not", "nothing", "now", "nowhere", "of", "off", "often", "on", "once", "one", "only", "onto", "or", "other", "others", "otherwise", "our", "ours", "ourselves", "out", "over", "own", "part", "per", "perhaps", "please", "put", "rather", "re", "same", "see", "seem", "seemed", "seeming", "seems", "serious", "several", "she", "should", "show", "side", "since", "sincere", "six", "sixty", "so", "some", "somehow", "someone", "something", "sometime", "sometimes", "somewhere", "still", "such", "system", "take", "ten", "than", "that", "the", "their", "them", "themselves", "then", "thence", "there", "thereafter", "thereby", "therefore", "therein", "thereupon", "these", "they", "thickv", "thin", "third", "this", "those", "though", "three", "through", "throughout", "thru", "thus", "to", "together", "too", "top", "toward", "towards", "twelve", "twenty", "two", "un", "under", "until", "up", "upon", "us", "very", "via", "was", "we", "well", "were", "what", "whatever", "when", "whence", "whenever", "where", "whereafter", "whereas", "whereby", "wherein", "whereupon", "wherever", "whether", "which", "while", "whither", "who", "whoever", "whole", "whom", "whose", "why", "will", "with", "within", "without", "would", "yet", "you", "your", "yours", "yourself", "yourselves", "the", "i'm", "(l)" };


            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);
            doc = RemoveScripts(doc);

            var text = doc.DocumentNode.SelectNodes("//body//text()").Select(node => node.InnerText);

            StringBuilder output = new StringBuilder();
            foreach (string line in text)
            {
                output.AppendLine(line);
            }
            string textOnly = HttpUtility.HtmlDecode(output.ToString());
            textOnly = Regex.Replace(textOnly, @"\s+", " ", RegexOptions.Multiline);

            Dictionary<string, int> dicWordCount = new Dictionary<string, int>();
            Dictionary<string, int> newCount = new Dictionary<string, int>();

            foreach (var item in textOnly.ToString().Split(' '))
            {
                var wordLower = item.Trim().ToLowerInvariant();
                wordLower = Regex.Replace(wordLower, @"\d", "");

                if (wordLower.Length < 3 || wordLower.Contains("-") || wordLower.Contains("_") || wordLower.Contains("-icon-") || wordLower.Contains("logo-")) continue;

                if (dicWordCount.ContainsKey(wordLower))
                {
                    int count = dicWordCount[wordLower] + 1;
                    dicWordCount[wordLower] = count;
                }
                else
                {
                    dicWordCount.Add(wordLower, 1);
                }
            }

            foreach (var subItem in marks)
            {
                dicWordCount.Remove(subItem);
            }

            var items = from pair in dicWordCount
                        orderby pair.Value descending,
                                pair.Key
                        select pair;
            return items;
        }
        private static HtmlDocument RemoveScripts(HtmlDocument webDocument)
        { 
            HtmlNodeCollection Nodes = webDocument.DocumentNode.SelectNodes("//script");
             
            if (Nodes == null)
                return webDocument;
             
            foreach (HtmlNode node in Nodes)
                node.Remove();

            return webDocument;

        }
    }
}
