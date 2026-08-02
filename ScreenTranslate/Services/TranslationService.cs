using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace ScreenTranslate.Services
{
    public sealed class TranslationService : ITranslationService, IDisposable
    {
        private readonly HttpClient _client = new HttpClient { Timeout = TimeSpan.FromSeconds(12) };
        public TranslationService() { _client.DefaultRequestHeaders.UserAgent.ParseAdd("ScreenTranslate/0.2"); }

        public async Task<string> TranslateToArabicAsync(string text, string sourceLanguageCode)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            var source = string.IsNullOrWhiteSpace(sourceLanguageCode) || sourceLanguageCode == "auto" ? "en" : sourceLanguageCode;
            var uri = "https://api.mymemory.translated.net/get?q=" + Uri.EscapeDataString(text) + "&langpair=" + Uri.EscapeDataString(source + "|ar");
            try
            {
                using (var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        var payload = new DataContractJsonSerializer(typeof(MyMemoryResponse)).ReadObject(stream) as MyMemoryResponse;
                        var translated = payload != null && payload.ResponseData != null ? payload.ResponseData.TranslatedText : null;
                        if (string.IsNullOrWhiteSpace(translated)) throw new TranslationException("لم تُرجع خدمة الترجمة نتيجة.");
                        return translated.Trim();
                    }
                }
            }
            catch (TranslationException) { throw; }
            catch (Exception ex)
            { if(ex is HttpRequestException || ex is TaskCanceledException || ex is IOException) throw new TranslationException("تعذر الاتصال بخدمة الترجمة. تحقق من اتصال الإنترنت وحاول مرة أخرى.", ex); throw; }
        }
        public void Dispose() { _client.Dispose(); }
        [DataContract] private sealed class MyMemoryResponse { [DataMember(Name = "responseData")] public ResponseData ResponseData { get; set; } }
        [DataContract] private sealed class ResponseData { [DataMember(Name = "translatedText")] public string TranslatedText { get; set; } }
    }

    public sealed class TranslationException : Exception
    {
        public TranslationException(string message) : base(message) { }
        public TranslationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
