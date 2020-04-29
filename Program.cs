using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Perimetre1
{
    class Program
    {
        private static HttpClient _client;
        private static string path= @"E:\C#\TrustablePerimeter\Perimetre1\" + "data.txt";

        static async Task Main(string[] args)
        {
            File.Delete(path);
            File.Create(path).Close();

            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:12345/");

            Dictionary<String, String> target = new Dictionary<String, String>()
            {
                {"customers", String.Empty  },
                { "accounts", String.Empty },
                {"customer/create", "id=7&name=Jean&account=7" },
                {"account/create", "id=8&customer=1"  },
                { "account/withdraw", "id=7&amount=100" }
            };

            await RequestPost(target);

            string content = File.ReadAllText(path);
            Console.WriteLine(content);
        }
        static async Task RequestPost(Dictionary<String, String> target)
        {
            foreach (KeyValuePair<String, String> item in target)
            {
                var request = new HttpRequestMessage(HttpMethod.Post, item.Key);
                request.Content = new StringContent(item.Value, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await _client.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();

                if (result.Equals(String.Empty))
                {
                    var valuePost = await request.Content.ReadAsStringAsync();
                    var newValue = valuePost.Replace("&", "\t").Replace("=", " = ");

                    WriteFile(response.RequestMessage.RequestUri.ToString() + "\n" + newValue + "\n" + response.StatusCode + "\n", path);
                }
                else
                {
                    WriteFile(result + "\n\n", path);
                }
            }
        }

        static void WriteFile(string contents, string path)
        {
            StreamWriter streamWriter = new StreamWriter(path, true);
            streamWriter.Write(contents);
            streamWriter.Close();
        }
    }
}
