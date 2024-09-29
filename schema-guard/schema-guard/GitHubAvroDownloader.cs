
using Octokit;
using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace schema_guard
{
    public class GitHubAvroDownloader
    {
        private static readonly string _githubToken = "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";
        private static readonly string _owner = "chyltonmr";
        private static readonly string _repo = "Quickforms";
        private static readonly string _path = "package.json";



        public async Task<string> GetAvroFileFromGitHub()
        {
            var client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _githubToken);

            //var url = $"https://api.github.com/repos/{_owner}/{_repo}/contents/{_path}";
            var url = $"https://api.github.com/repos/chyltonmr/Quickforms/contents/package.json";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsByteArrayAsync();
                var base64Content = jsonResponse.ToString();
                var avroContent = Convert.FromBase64String(base64Content);
                return Encoding.UTF8.GetString(avroContent);
            }

            return "";
        }
    }
}



//LIB PARA ACESSAR GIT HUB DE FORMA SIMPLES:
//https://github.com/octokit/octokit.net?tab=readme-ov-file
//DOD DA LIB: https://octokitnet.readthedocs.io/en/latest/