using System.Security.Claims;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Api.Swagger.Examples.Career;
using ReStartAI.Application.IoT;
using ReStartAI.Application.Matching;
using ReStartAI.Application.Parsing;
using ReStartAI.Application.Services;
using ReStartAI.Application.WhyMe;
using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using UglyToad.PdfPig;

namespace ReStartAI.Api.Controllers.Career
{
    [ApiController]
    [Route("api")]
    [Authorize]
    [Produces("application/json")]
    public class CareerController : ControllerBase
    {
        private readonly ICurriculoRepository _curriculos;
        private readonly IVagaRepository _vagas;
        private readonly InsightClient _insightsClient;
        private readonly InsightTriggerService _insightTrigger;
        private readonly INotificacaoRepository _notificacoes;
        private readonly WhyMeGenerator _whyMe;

        public CareerController(
            ICurriculoRepository curriculos,
            IVagaRepository vagas,
            InsightClient insightsClient,
            InsightTriggerService insightTrigger,
            INotificacaoRepository notificacoes,
            WhyMeGenerator whyMe)
        {
            _curriculos = curriculos;
            _vagas = vagas;
            _insightsClient = insightsClient;
            _insightTrigger = insightTrigger;
            _notificacoes = notificacoes;
            _whyMe = whyMe;
        }

        public record UploadCurriculoResponse(
            string CurriculoId,
            string? Nome,
            string? Email,
            string? Telefone,
            List<string> Skills,
            List<string> Areas,
            List<string> PapeisSugeridos
        );

        public record TriggerRequest(
            MetricsDto Metrics,
            List<string> LastEvents,
            ProfileDto Profile,
            BestOpportunityDto? BestOpportunity
        );

        public record TriggerResponse(
            string NotificationId,
            string Insight,
            string ActionTag
        );

        public record AutoTriggerResponse(
            string NotificationId,
            string Insight,
            string ActionTag
        );

        public record MelhorMatchResponse(
            string Role,
            int MatchPercent,
            List<string> WhyMe,
            List<string> MissingSkills
        );

        [HttpPost("curriculos")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(
            Summary = "Faz upload do currículo em PDF",
            Description = "Recebe o arquivo PDF do currículo, extrai o texto e skills e salva o currículo vinculado ao usuário logado."
        )]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(UploadCurriculoResponseExample))]
        [ProducesResponseType(typeof(UploadCurriculoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UploadCurriculoResponse>> UploadCurriculo([FromForm] IFormFile file)
        {
            var uid = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(uid))
                return Unauthorized();

            if (file == null || file.Length == 0)
                return BadRequest("Arquivo de currículo é obrigatório.");

            string texto;
            using (var doc = PdfDocument.Open(file.OpenReadStream()))
            {
                var sb = new StringBuilder();
                foreach (var page in doc.GetPages())
                {
                    sb.AppendLine(page.Text);
                }

                texto = sb.ToString();
            }

            var parser = new ResumeParser();
            var parsed = parser.Parse(texto);

            var curriculo = new Curriculo
            {
                UsuarioId = uid,
                NomeArquivo = file.FileName,
                Texto = texto,
                Skills = parsed.Skills,
                CriadoEm = DateTime.UtcNow
            };

            await _curriculos.CreateAsync(curriculo);

            var response = new UploadCurriculoResponse(
                curriculo.Id,
                parsed.Nome,
                parsed.Email,
                parsed.Telefone,
                parsed.Skills,
                parsed.Areas,
                parsed.PapeisSugeridos
            );

            return Created($"api/curriculos/{curriculo.Id}", response);
        }

        [HttpPost("insights/trigger")]
        [SwaggerOperation(
            Summary = "Gera um insight manual a partir das métricas IoT do usuário",
            Description = "Recebe métricas, últimos eventos, perfil e melhor oportunidade e chama o serviço IoT com IA generativa para produzir um insight curto e acionável."
        )]
        [SwaggerRequestExample(typeof(TriggerRequest), typeof(TriggerRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TriggerResponseExample))]
        [ProducesResponseType(typeof(TriggerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TriggerResponse>> Trigger([FromBody] TriggerRequest req)
        {
            var uid = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(uid))
                return Unauthorized();

            var payload = new InsightRequestDto(
                uid,
                req.Metrics,
                req.LastEvents,
                req.Profile,
                req.BestOpportunity
            );

            var result = await _insightsClient.GetInsightAsync(payload, HttpContext.RequestAborted);

            var titulo = req.BestOpportunity?.Role switch
            {
                not null when result.actionTag == "APPLY_FOCUS" => "Foque em candidaturas com alto fit",
                not null when result.actionTag == "EXPLORE_JOBS" => "Explore novas oportunidades sugeridas",
                not null when result.actionTag == "STUDY_GAP" => "Invista nos seus gaps para evoluir",
                _ => "Novo insight sobre sua jornada"
            };

            var notification = new Notificacao
            {
                UsuarioId = uid,
                Titulo = titulo,
                Mensagem = result.insight
            };

            await _notificacoes.CreateAsync(notification);

            return Ok(new TriggerResponse(notification.Id, result.insight, result.actionTag));
        }

        [HttpPost("insights/auto")]
        [SwaggerOperation(
            Summary = "Gera um insight automático a partir das métricas e currículo do usuário",
            Description = "Monta o payload automaticamente com base nos eventos recentes, currículo e vagas disponíveis e chama o serviço IoT com IA generativa."
        )]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AutoTriggerResponseExample))]
        [ProducesResponseType(typeof(AutoTriggerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AutoTriggerResponse>> Auto()
        {
            var uid = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(uid))
                return Unauthorized();

            var payload = await _insightTrigger.BuildAsync(uid);
            if (payload is null)
                return NotFound();

            var result = await _insightsClient.GetInsightAsync(payload, HttpContext.RequestAborted);

            var titulo = payload.BestOpportunity?.Role switch
            {
                not null when result.actionTag == "APPLY_FOCUS" => "Oportunidade com alto match para você",
                not null when result.actionTag == "EXPLORE_JOBS" => "Explore novas vagas sugeridas",
                not null when result.actionTag == "STUDY_GAP" => "Sugestão para fechar seus gaps",
                _ => "Novo insight automático"
            };

            var notification = new Notificacao
            {
                UsuarioId = uid,
                Titulo = titulo,
                Mensagem = result.insight
            };

            await _notificacoes.CreateAsync(notification);

            return Ok(new AutoTriggerResponse(notification.Id, result.insight, result.actionTag));
        }

        [HttpGet("usuarios/me/melhor-match")]
        [SwaggerOperation(
            Summary = "Retorna a vaga com melhor match para o usuário logado",
            Description = "Usa as skills do currículo e as vagas cadastradas para calcular o melhor match, listar por que você é um bom fit e apontar gaps principais."
        )]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MelhorMatchResponseExample))]
        [ProducesResponseType(typeof(MelhorMatchResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MelhorMatchResponse>> MelhorMatch()
        {
            var uid = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(uid))
                return Unauthorized();

            var curriculosUsuario = await _curriculos.GetAllAsync(1, 10);
            var curriculo = curriculosUsuario.FirstOrDefault(c => c.UsuarioId == uid);
            if (curriculo is null)
                return NotFound();

            var vagas = await _vagas.GetAllAsync(1, 50);
            var matcher = new DeterministicMatcher();
            var best = matcher.BestMatch(vagas, curriculo.Skills);
            if (best is null)
                return NotFound();

            var why = await _whyMe.GenerateAsync(
                best.Vaga.Titulo,
                curriculo.Skills,
                best.MustMatched,
                best.NiceMatched,
                best.MustMissing,
                best.NiceMissing
            );

            var response = new MelhorMatchResponse(
                best.Vaga.Titulo,
                (int)Math.Round(best.Percentual * 100),
                why,
                best.MustMissing
            );

            return Ok(response);
        }
    }
}
