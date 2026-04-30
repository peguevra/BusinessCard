using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class SupabaseService
{
    private readonly HttpClient _http;

    private const string Url = "https://ylnykglsckcfgalvefcy.supabase.co";
    private const string Key = "sb_publishable_eYNsMIHImr5MhyOpeBW-eg_D42B08FA";

    public SupabaseService()
    {
        _http = new HttpClient();

        _http.DefaultRequestHeaders.Add("apikey", Key);
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", Key);
    }

    // =========================
    // INSERT
    // =========================
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

    // =========================
    // SELECT ALL（追加）
    // =========================
    public async Task<List<BuisicessCardRecord>> GetAll()
    {
        var res = await _http.GetAsync($"{Url}/rest/v1/cards?select=*");

        if (!res.IsSuccessStatusCode)
        {
            var err = await res.Content.ReadAsStringAsync();
            throw new Exception("Supabase取得失敗: " + err);
        }

        var json = await res.Content.ReadAsStringAsync();

        var data = JsonSerializer.Deserialize<List<Dto>>(json);

        return data.Select(x => new BuisicessCardRecord
        {
            Category = x.category ?? "",
            Name = x.name ?? "",
            Url = x.url ?? "",
            CreatedAt = DateTime.Parse(x.date)
        }).ToList();
    }

    // DTO
    private class Dto
    {
        public string date { get; set; }
        public string category { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
}