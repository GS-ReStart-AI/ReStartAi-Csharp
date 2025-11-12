using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ReStartAI.Application.WhyMe;

public class WhyMeGenerator
{
    private readonly IConfiguration _cfg;
    private readonly HttpClient _http;

    public WhyMeGenerator(IConfiguration cfg, HttpClient http)
    {
        _cfg = cfg;
        _http = http;
    }

    public async Task<List<string>> GenerateAsync(
        string papel,
        IEnumerable<string> userSkills,
        IEnumerable<string> mustMatched,
        IEnumerable<string> niceMatched,
        IEnumerable<string> mustMissing,
        IEnumerable<string> niceMissing)
    {
        var apiKey = _cfg["OpenAI:ApiKey"];
        var model = _cfg["OpenAI:Model"] ?? "gpt-4o-mini";
        if (string.IsNullOrWhiteSpace(apiKey))
            return FallbackBullets(mustMatched, niceMatched);

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var prompt = BuildPrompt(papel, userSkills, mustMatched, niceMatched, mustMissing, niceMissing);

        var payload = new
        {
            model,
            messages = new object[]
            {
                new { role = "system", content = "Você é um assistente que escreve em português do Brasil. Responda apenas com 2 a 3 bullets curtos e objetivos explicando por que a pessoa é boa para o papel sugerido." },
                new { role = "user", content = prompt }
            },
            temperature = 0.3,
            max_tokens = 180
        };

        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            using var resp = await _http.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                return FallbackBullets(mustMatched, niceMatched);

            using var doc = JsonDocument.Parse(body);
            var msg = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
            var lines = msg.Split('\n')
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => l.TrimStart('-', '*', '•', '–', '—').Trim())
                .Where(l => l.Length > 0)
                .Take(3)
                .ToList();

            if (lines.Count == 0)
                return FallbackBullets(mustMatched, niceMatched);

            return lines;
        }
        catch
        {
            return FallbackBullets(mustMatched, niceMatched);
        }
    }

    private static string BuildPrompt(
        string papel,
        IEnumerable<string> userSkills,
        IEnumerable<string> mustMatched,
        IEnumerable<string> niceMatched,
        IEnumerable<string> mustMissing,
        IEnumerable<string> niceMissing)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Papel sugerido: {papel}");
        sb.AppendLine($"Skills do usuário: {string.Join(", ", userSkills)}");
        sb.AppendLine($"Obrigatórias atendidas: {string.Join(", ", mustMatched)}");
        sb.AppendLine($"Obrigatórias faltantes: {string.Join(", ", mustMissing)}");
        sb.AppendLine($"Desejáveis atendidas: {string.Join(", ", niceMatched)}");
        sb.AppendLine($"Desejáveis faltantes: {string.Join(", ", niceMissing)}");
        sb.AppendLine("Escreva apenas 2 a 3 bullets curtos e objetivos explicando por que a pessoa é adequada. Evite repetições, jargões e frases longas.");
        return sb.ToString();
    }

    private static List<string> FallbackBullets(IEnumerable<string> mustMatched, IEnumerable<string> niceMatched)
    {
        var mm = mustMatched.Take(2).ToList();
        var nm = niceMatched.Take(2).ToList();
        var bullets = new List<string>();
        if (mm.Count > 0) bullets.Add($"Atende requisitos-chave: {string.Join(", ", mm)}.");
        if (nm.Count > 0) bullets.Add($"Complementos valorizados: {string.Join(", ", nm)}.");
        if (bullets.Count == 0) bullets.Add("Histórico e habilidades alinhados ao papel sugerido.");
        return bullets;
    }
}
