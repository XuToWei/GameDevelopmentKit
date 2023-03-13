using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QFSW.QC.Extras
{
    [CommandPrefix("http.")]
    public static class HttpCommands
    {
        private static readonly HttpClient _client = new HttpClient();

        [Command("get", "Sends a GET request to the specified URL.")]
        private static async Task<string> Get(string url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);

            return await response.Content.ReadAsStringAsync();
        }

        [Command("delete", "Sends a DELETE request to the specified URL.")]
        private static async Task<string> Delete(string url)
        {
            HttpResponseMessage response = await _client.DeleteAsync(url);

            return await response.Content.ReadAsStringAsync();
        }

        [Command("post", "Sends a POST request to the specified URL. " +
                         "A body may be sent with the request, with a default mediaType of text/plain.")]
        private static async Task<string> Post(string url, string content = "", string mediaType = "text/plain")
        {
            HttpContent body = new StringContent(content, Encoding.Default, mediaType);
            HttpResponseMessage response = await _client.PostAsync(url, body);

            return await response.Content.ReadAsStringAsync();
        }

        [Command("put", "Sends a PUT request to the specified URL. " +
                        "A body may be sent with the request, with a default mediaType of text/plain.")]
        private static async Task<string> Put(string url, string content = "", string mediaType = "text/plain")
        {
            HttpContent body = new StringContent(content, Encoding.Default, mediaType);
            HttpResponseMessage response = await _client.PutAsync(url, body);

            return await response.Content.ReadAsStringAsync();
        }
    }
}