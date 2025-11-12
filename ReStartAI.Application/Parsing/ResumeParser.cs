using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ReStartAI.Application.Parsing;

public record ParseResult(string? Nome, string? Email, string? Telefone, List<string> Skills, List<string> Areas, List<string> PapeisSugeridos);

public class ResumeParser
{
    private static readonly Regex RxEmail = new(@"[A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex RxPhone = new(@"\+?\d[\d\s().-]{7,}\d", RegexOptions.Compiled);
    private static readonly Regex RxNameLine = new(@"^[A-Za-zÀ-ÖØ-öø-ÿ][A-Za-zÀ-ÖØ-öø-ÿ'`\-]+\s+[A-Za-zÀ-ÖØ-öø-ÿ][A-Za-zÀ-ÖØ-öø-ÿ'`\-]+.*$", RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly string[] KnownSkills =
    {
        "comunicacao","empatia","atendimento","telemarketing","suporte","pdv","caixa","vendas","negociacao","prospeccao","crm",
        "estoque","reposicao","logistica","almoxarifado","expedicao","roteirizacao","motorista","entrega","empilhadeira",
        "administrativo","recepcao","secretariado","agendamento","arquivo","office","word","excel","powerpoint",
        "contabilidade","fiscal","faturamento","financeiro","concilicacao","contaspagar","contasreceber","nota","nf",
        "rh","dp","folha","beneficios","recrutamento","selecao","treinamento",
        "marketing","social","socialmedia","conteudo","copywriting","trafego","seo","sem","googleads","metaads",
        "design","photoshop","illustrator","canva","video","edicao","premiere","capcut",
        "dados","sql","excel","python","powerbi",
        "csharp","dotnet","javascript","react","node","dart","flutter","git","rest","firebase","ui",
        "linux","docker","kubernetes","cloud","azure","aws","gcp",
        "nlp","ml","ia",
        "saude","enfermagem","tecnicoenfermagem","cuidador","farmacia","balconista",
        "educacao","professor","monitor","pedagogia","alfabetizacao",
        "juridico","processos","peticoes","contratos","oab",
        "construcao","pedreiro","eletricista","hidraulica","pintura",
        "manutencao","mecanico","solda","torneiro",
        "seguranca","vigilante","cftv","portaria","prevencao",
        "gastronomia","cozinha","auxiliardecozinha","garcom","barista",
        "limpeza","zeladoria","jardinagem","hotelaria","recepcionista"
    };

    private static readonly Dictionary<string,string> SkillArea = new(StringComparer.OrdinalIgnoreCase)
    {
        ["comunicacao"]="Atendimento",
        ["empatia"]="Atendimento",
        ["atendimento"]="Atendimento",
        ["telemarketing"]="Atendimento",
        ["suporte"]="Atendimento",
        ["pdv"]="Varejo",
        ["caixa"]="Varejo",
        ["vendas"]="Comercial",
        ["negociacao"]="Comercial",
        ["prospeccao"]="Comercial",
        ["crm"]="Comercial",
        ["estoque"]="Logistica",
        ["reposicao"]="Logistica",
        ["logistica"]="Logistica",
        ["almoxarifado"]="Logistica",
        ["expedicao"]="Logistica",
        ["roteirizacao"]="Logistica",
        ["motorista"]="Logistica",
        ["entrega"]="Logistica",
        ["empilhadeira"]="Logistica",
        ["administrativo"]="Administrativo",
        ["recepcao"]="Administrativo",
        ["secretariado"]="Administrativo",
        ["agendamento"]="Administrativo",
        ["arquivo"]="Administrativo",
        ["office"]="Administrativo",
        ["word"]="Administrativo",
        ["excel"]="Dados",
        ["powerpoint"]="Administrativo",
        ["contabilidade"]="Financeiro",
        ["fiscal"]="Financeiro",
        ["faturamento"]="Financeiro",
        ["financeiro"]="Financeiro",
        ["concilicacao"]="Financeiro",
        ["contaspagar"]="Financeiro",
        ["contasreceber"]="Financeiro",
        ["nota"]="Financeiro",
        ["nf"]="Financeiro",
        ["rh"]="RH",
        ["dp"]="RH",
        ["folha"]="RH",
        ["beneficios"]="RH",
        ["recrutamento"]="RH",
        ["selecao"]="RH",
        ["treinamento"]="RH",
        ["marketing"]="Marketing",
        ["social"]="Marketing",
        ["socialmedia"]="Marketing",
        ["conteudo"]="Marketing",
        ["copywriting"]="Marketing",
        ["trafego"]="Marketing",
        ["seo"]="Marketing",
        ["sem"]="Marketing",
        ["googleads"]="Marketing",
        ["metaads"]="Marketing",
        ["design"]="Marketing",
        ["photoshop"]="Marketing",
        ["illustrator"]="Marketing",
        ["canva"]="Marketing",
        ["video"]="Marketing",
        ["edicao"]="Marketing",
        ["premiere"]="Marketing",
        ["capcut"]="Marketing",
        ["dados"]="Dados",
        ["sql"]="Dados",
        ["python"]="Dados",
        ["powerbi"]="Dados",
        ["csharp"]="Desenvolvimento",
        ["dotnet"]="Desenvolvimento",
        ["javascript"]="Desenvolvimento",
        ["react"]="Desenvolvimento",
        ["node"]="Desenvolvimento",
        ["dart"]="Desenvolvimento",
        ["flutter"]="Desenvolvimento",
        ["git"]="Desenvolvimento",
        ["rest"]="Desenvolvimento",
        ["firebase"]="Desenvolvimento",
        ["ui"]="Desenvolvimento",
        ["linux"]="Infraestrutura",
        ["docker"]="Infraestrutura",
        ["kubernetes"]="Infraestrutura",
        ["cloud"]="Infraestrutura",
        ["azure"]="Infraestrutura",
        ["aws"]="Infraestrutura",
        ["gcp"]="Infraestrutura",
        ["nlp"]="IA",
        ["ml"]="IA",
        ["ia"]="IA",
        ["saude"]="Saude",
        ["enfermagem"]="Saude",
        ["tecnicoenfermagem"]="Saude",
        ["cuidador"]="Saude",
        ["farmacia"]="Saude",
        ["balconista"]="Varejo",
        ["educacao"]="Educacao",
        ["professor"]="Educacao",
        ["monitor"]="Educacao",
        ["pedagogia"]="Educacao",
        ["alfabetizacao"]="Educacao",
        ["juridico"]="Juridico",
        ["processos"]="Juridico",
        ["peticoes"]="Juridico",
        ["contratos"]="Juridico",
        ["oab"]="Juridico",
        ["construcao"]="Construcao",
        ["pedreiro"]="Construcao",
        ["eletricista"]="Construcao",
        ["hidraulica"]="Construcao",
        ["pintura"]="Construcao",
        ["manutencao"]="Manutencao",
        ["mecanico"]="Manutencao",
        ["solda"]="Manutencao",
        ["torneiro"]="Manutencao",
        ["seguranca"]="Seguranca",
        ["vigilante"]="Seguranca",
        ["cftv"]="Seguranca",
        ["portaria"]="Seguranca",
        ["prevencao"]="Seguranca",
        ["gastronomia"]="Gastronomia",
        ["cozinha"]="Gastronomia",
        ["auxiliardecozinha"]="Gastronomia",
        ["garcom"]="Gastronomia",
        ["barista"]="Gastronomia",
        ["limpeza"]="ServicosGerais",
        ["zeladoria"]="ServicosGerais",
        ["jardinagem"]="ServicosGerais",
        ["hotelaria"]="Hospitalidade",
        ["recepcionista"]="Hospitalidade"
    };

    private static readonly Dictionary<string,string[]> AreaToRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Atendimento"] = new[] { "Agente de Atendimento", "Suporte ao Cliente", "Operador de SAC" },
        ["Comercial"] = new[] { "Assistente Comercial", "Vendedor Interno", "SDR Júnior" },
        ["Varejo"] = new[] { "Atendente de Loja", "Operador de Caixa", "Repositor" },
        ["Logistica"] = new[] { "Auxiliar de Logística", "Assistente de Expedição", "Auxiliar de Almoxarifado" },
        ["Administrativo"] = new[] { "Assistente Administrativo", "Recepcionista", "Secretária(o)" },
        ["Financeiro"] = new[] { "Assistente Financeiro", "Auxiliar de Faturamento", "Auxiliar Contábil" },
        ["RH"] = new[] { "Assistente de RH", "Auxiliar de DP", "Recrutador Júnior" },
        ["Marketing"] = new[] { "Assistente de Marketing", "Social Media Júnior", "Redator Júnior" },
        ["Dados"] = new[] { "Analista de Dados Jr.", "BI Jr.", "Assistente de DataOps" },
        ["Desenvolvimento"] = new[] { "Dev Mobile Jr.", "Dev Backend Jr.", "Dev Frontend Jr." },
        ["Infraestrutura"] = new[] { "Suporte Técnico", "DevOps Jr.", "SysAdmin Jr." },
        ["IA"] = new[] { "Assistente de IA Jr.", "Analista de NLP Jr.", "MLOps Jr." },
        ["Saude"] = new[] { "Auxiliar de Enfermagem", "Cuidador", "Atendente de Farmácia" },
        ["Educacao"] = new[] { "Auxiliar de Classe", "Monitor Educacional", "Professor(a) Auxiliar" },
        ["Juridico"] = new[] { "Assistente Jurídico", "Auxiliar de Escritório Jurídico", "Estagiário(a) Direito" },
        ["Construcao"] = new[] { "Ajudante de Obras", "Oficial de Manutenção", "Eletricista Predial" },
        ["Manutencao"] = new[] { "Mecânico Auxiliar", "Soldador Auxiliar", "Torneiro Auxiliar" },
        ["Seguranca"] = new[] { "Vigilante", "Porteiro", "Monitor de CFTV" },
        ["Gastronomia"] = new[] { "Auxiliar de Cozinha", "Garçom/Garçonete", "Atendente de Lanchonete" },
        ["ServicosGerais"] = new[] { "Auxiliar de Limpeza", "Zelador", "Jardineiro" },
        ["Hospitalidade"] = new[] { "Recepcionista de Hotel", "Camareiro(a)", "Atendente de Hospedagem" }
    };

    public ParseResult Parse(string texto)
    {
        var email = RxEmail.Match(texto).Success ? RxEmail.Match(texto).Value : null;
        var phone = RxPhone.Match(texto).Success ? NormalizePhone(RxPhone.Match(texto).Value) : null;

        string? nome = null;
        var m = RxNameLine.Match(texto);
        if (m.Success)
        {
            var line = m.Value.Trim();
            if (!line.Contains('@') && !Regex.IsMatch(line, @"\d")) nome = line.Split('\n')[0].Trim();
        }

        var skills = DetectSkills(texto);
        var areas = InferAreas(skills);
        var roles = SuggestRoles(areas);

        return new ParseResult(nome, email, phone, skills, areas, roles);
    }

    private static string Normalize(string s)
    {
        var nf = s.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var ch in nf)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (uc != UnicodeCategory.NonSpacingMark) sb.Append(ch);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
    }

    private static List<string> DetectSkills(string texto)
    {
        var norm = Normalize(texto).Replace(" ", "");
        var found = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var sk in KnownSkills)
        {
            var token = sk.ToLowerInvariant();
            if (Regex.IsMatch(norm, $@"\b{Regex.Escape(token)}\b", RegexOptions.IgnoreCase))
                found.Add(sk);
        }
        return found.OrderBy(x => x).ToList();
    }

    private static List<string> InferAreas(IEnumerable<string> skills)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var s in skills)
            if (SkillArea.TryGetValue(s, out var area)) set.Add(area);
        if (set.Count == 0) set.Add("Comercial");
        return set.OrderBy(x => x).ToList();
    }

    private static List<string> SuggestRoles(IEnumerable<string> areas)
    {
        var result = new List<string>();
        foreach (var a in areas)
        {
            if (AreaToRoles.TryGetValue(a, out var roles))
                result.AddRange(roles);
        }
        return result.Distinct(StringComparer.OrdinalIgnoreCase).Take(3).ToList();
    }

    private static string NormalizePhone(string raw)
    {
        var digits = new string(raw.Where(char.IsDigit).ToArray());
        return digits;
    }
}
