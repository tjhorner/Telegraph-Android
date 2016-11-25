using System.Threading.Tasks;
using System.Net.Http;
using ModernHttpClient;
using System.Threading;
using Android.Util;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;

namespace Telegraph
{
    class TelegraphApi
    {
        public static string API_BASE = "https://edit.telegra.ph/";

        public static string PostToJson(string post)
        {
            JArray elements = new JArray();

            // TODO make this more abstract and shit
            
            foreach(string line in post.Split('\n'))
            {
                JObject lineEl = new JObject();

                lineEl["_"] = "p"; // element tag

                JArray elContents = new JArray();
                JObject elText = new JObject();

                elText["t"] = line;

                elContents.Add(elText);

                lineEl["c"] = elContents;

                elements.Add(lineEl);
            }

            return JsonConvert.SerializeObject(elements);

            //foreach(var node in html.DocumentNode.ChildNodes)
            //{
            //    JObject el = new JObject();
            //    el["_"] = node.Name;

            //    JArray content = new JArray();

            //    JObject elContent = new JObject();
            //    elContent["t"] = node.InnerText;

            //    content.Add(elContent);

            //    elements.Add(el);
            //}

            //return elements.ToString();
        }

        public static string BuildSpecialBody(string title, string author, string post, string pageId)
        {
            string boundary = "-----------------------------TelegraPhBoundary21";
            string body = boundary;

            body += "\nContent-Disposition: form-data; name=\"Data\";filename=\"content.html\"\nContent-Type: plain/text\n\n";
            body += post + "\n" + boundary;

            body += "\nContent-Disposition: form-data; name=\"title\"\n\n";
            body += title + "\n" + boundary;

            body += "\nContent-Disposition: form-data; name=\"author\"\n\n";
            body += author + "\n" + boundary;

            body += "\nContent-Disposition: form-data; name=\"page_id\"\n\n";
            body += pageId + "\n" + boundary + "--";

            return body;
        }

        public static async Task<SaveResponse> PostSave(string title, string author, string post)
        {
            HttpClient client = new HttpClient(new NativeMessageHandler());
            string requestUri = API_BASE + "save";
            //string requestUri = "http://requestb.in/pnnhrvpn";
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            string body = BuildSpecialBody(title, author, PostToJson(post), "0");

            StringContent content = new StringContent(body, Encoding.UTF8, "multipart/form-data");
            content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=---------------------------TelegraPhBoundary21");
            content.Headers.Add("Origin", "http://telegra.ph");
            content.Headers.TryAddWithoutValidation("Referer", "http://telegra.ph/");
            content.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36");

            //StringContent content = new StringContent(BuildSpecialBody(title, author, post, "0"));
            //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data; boundary=---------------------------TelegraPhBoundary21");

            //MultipartFormDataContent content = new MultipartFormDataContent();
            //content.Add(new StringContent(title), "title");
            //content.Add(new StringContent(author), "author");
            //content.Add(new StringContent(post), "Html");
            //content.Add(new StringContent("0"), "page_id");

            Log.Debug("TELEGRAPH", BuildSpecialBody(title, author, post, "0"));

            var res = await client.PostAsync(requestUri, content, cancellationTokenSource.Token);

            string resText = await res.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SaveResponse>(resText);
        }
    }
}