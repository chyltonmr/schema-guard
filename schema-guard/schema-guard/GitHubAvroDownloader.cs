using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace schema_guard
{
    public class GitHubAvroDownloader
    {
        private static readonly string _githubToken = "YOUR_GITHUB_TOKEN";
        private static readonly string _owner = "chyltonmr";
        private static readonly string _repo = "schema-guard";
        private static readonly string _path = "path_to_avro_file.avro";



        public static async Task<string> GetAvroFileFromGitHub()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", _githubToken);

            var url = $"https://api.github.com/repos/{_owner}/{_repo}/contents/{_path}";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStreamAsync();
                var base64Content = jsonResponse.ToString();
                var avroContent = Convert.FromBase64String(base64Content);
                return Encoding.UTF8.GetString(avroContent);
            }

            return null;
        }
    }
}

