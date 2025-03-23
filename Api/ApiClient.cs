using Newtonsoft.Json;
using System.Drawing.Imaging;
using System.Net.Http.Headers;
using System.Text;

namespace ClickMashine.Api
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return $"Ошибка: {ex.Message}";
            }
        }

        public async Task<string> PostAsync<T>(string url, T data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return $"Ошибка: {ex.Message}";
            }
        }
        public async Task<string> SendBitmapsAsync(ImageDecode imageDecode)
        {
            using (var client = new HttpClient())
            using (var content = new MultipartFormDataContent())
            {
                // Добавляем категорию и тип сайта
                content.Add(new StringContent(imageDecode.ImageCategory.ToString()), "ImageCategory");
                content.Add(new StringContent(imageDecode.Site.ToString()), "Site");

                // Конвертируем Bitmaps в Stream и добавляем в запрос
                for (int i = 0; i < imageDecode.Files.Count(); i++)
                {
                    using var ms = new MemoryStream();
                    imageDecode.Files.ElementAt(i).Save(ms, ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);

                    var fileContent = new StreamContent(ms);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                    content.Add(fileContent, "Files", $"image_{i}.png");
                }

                var response = await client.PostAsync("https://your-api-endpoint/upload", content);
                response.EnsureSuccessStatusCode();

                string responseText = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ответ от сервера: {responseText}");
                return responseText;
            }
        }
    }
}
