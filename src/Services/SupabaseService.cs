using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class SupabaseService
{
    private readonly HttpClient _http;

    private const string Url = "https://YOUR_PROJECT_ID.supabase.co";
    private const string Key = "YOUR_PUBLISHABLE_KEY";

    public SupabaseService()
    {
        _http = new HttpClient();

        _http.DefaultRequestHeaders.Add("apikey", Key);
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", Key);
    }

    public async Task Insert(BuisicessCardRecord record)
    {
        var payload = new[]
        {
            new {
                date = record.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                category = record.Category,
                name = record.Name,
                url = record.Url
            }
        };

        var json = JsonSerializer.Serialize(payload);

        var res = await _http.PostAsync(
            $"{Url}/rest/v1/cards",
            new StringContent(json, Encoding.UTF8, "application/json")
        );

        if (!res.IsSuccessStatusCode)
        {
            var err = await res.Content.ReadAsStringAsync();
            throw new Exception("Supabase Insert失敗: " + err);
        }
    }
}