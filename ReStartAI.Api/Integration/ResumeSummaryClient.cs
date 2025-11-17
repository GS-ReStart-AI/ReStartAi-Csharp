using Microsoft.Extensions.Options;

namespace ReStartAI.Api.Integration
{
    public class IotOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string InternalKey { get; set; } = string.Empty;
    }

    public record ResumeSummaryResult(
        IReadOnlyList<string> Areas,
        string BestRole,
        IReadOnlyList<string> Roles,
        string Seniority,
        int YearsOfExperience,
        IReadOnlyList<string> SkillsDetected
    );

    public interface IResumeSummaryClient
    {
        Task<ResumeSummaryResult> GenerateAsync(string usuarioId, string curriculoTexto, CancellationToken ct = default);
    }

    public class ResumeSummaryClient : IResumeSummaryClient
    {
        private readonly HttpClient _http;
        private readonly IotOptions _options;

        public ResumeSummaryClient(HttpClient http, IOptions<IotOptions> options)
        {
            _http = http;
            _options = options.Value;
        }

        private sealed class JobSearchQueryDto
        {
            public string Title { get; set; } = string.Empty;
            public string Query { get; set; } = string.Empty;
            public List<string> Platforms { get; set; } = new();
        }

        private sealed class ResumeSummaryResponseDto
        {
            public List<string> Areas { get; set; } = new();
            public string Best_Role { get; set; } = string.Empty;
            public string BestRole { get; set; } = string.Empty;
            public List<string> Roles { get; set; } = new();
            public string Seniority { get; set; } = string.Empty;
            public int Years_Of_Experience { get; set; }
            public int YearsOfExperience { get; set; }
            public List<string> Skills_Detected { get; set; } = new();
            public List<string> SkillsDetected { get; set; } = new();
            public List<JobSearchQueryDto> Job_Search_Queries { get; set; } = new();
            public List<JobSearchQueryDto> JobSearchQueries { get; set; } = new();
        }

        public async Task<ResumeSummaryResult> GenerateAsync(string usuarioId, string curriculoTexto, CancellationToken ct = default)
        {
            var url = $"{_options.BaseUrl.TrimEnd('/')}/resume-summary";

            var payload = new
            {
                usuarioId = usuarioId,
                curriculoTexto = curriculoTexto
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(payload)
            };

            request.Headers.Add("X-Internal-Key", _options.InternalKey);

            using var response = await _http.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Erro ao chamar IoT resume-summary: {(int)response.StatusCode}");
            }

            var dto = await response.Content.ReadFromJsonAsync<ResumeSummaryResponseDto>(cancellationToken: ct);
            if (dto == null)
            {
                throw new HttpRequestException("Resposta vazia do IoT resume-summary");
            }

            var areas = dto.Areas ?? new List<string>();
            var roles = dto.Roles ?? new List<string>();

            var bestRole = !string.IsNullOrWhiteSpace(dto.BestRole)
                ? dto.BestRole
                : dto.Best_Role;

            if (string.IsNullOrWhiteSpace(bestRole) && roles.Count > 0)
            {
                bestRole = roles[0];
            }

            var years = dto.YearsOfExperience != 0
                ? dto.YearsOfExperience
                : dto.Years_Of_Experience;

            var skills = dto.SkillsDetected.Count > 0
                ? dto.SkillsDetected
                : dto.Skills_Detected;

            return new ResumeSummaryResult(
                Areas: areas,
                BestRole: bestRole ?? string.Empty,
                Roles: roles,
                Seniority: dto.Seniority ?? string.Empty,
                YearsOfExperience: years,
                SkillsDetected: skills
            );
        }
    }
}
