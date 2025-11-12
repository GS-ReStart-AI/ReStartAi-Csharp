using ReStartAI.Domain.Entities;

namespace ReStartAI.Application.Matching;

public record MatchDetails(
    Vaga Vaga,
    int Score,
    double Percentual,
    List<string> MustMatched,
    List<string> MustMissing,
    List<string> NiceMatched,
    List<string> NiceMissing
);

public class DeterministicMatcher
{
    public MatchDetails? BestMatch(IEnumerable<Vaga> vagas, IEnumerable<string> userSkills)
    {
        var skillSet = new HashSet<string>(userSkills.Select(s => s.Trim().ToLowerInvariant()));

        MatchDetails? best = null;

        foreach (var vaga in vagas)
        {
            var must = vaga.MustSkills.Select(s => s.Trim().ToLowerInvariant()).ToList();
            var nice = vaga.NiceSkills.Select(s => s.Trim().ToLowerInvariant()).ToList();

            var mustMatched = must.Where(skillSet.Contains).Distinct().ToList();
            var mustMissing = must.Except(mustMatched).Distinct().ToList();

            var niceMatched = nice.Where(skillSet.Contains).Distinct().ToList();
            var niceMissing = nice.Except(niceMatched).Distinct().ToList();

            var baseMax = must.Count * 2 + nice.Count * 1;
            var raw = mustMatched.Count * 2 + niceMatched.Count * 1;
            var penalty = mustMissing.Count * 1;
            var score = raw - penalty;
            var percent = baseMax > 0 ? Math.Max(0, Math.Min(1, (double)raw / baseMax)) : 0;

            var details = new MatchDetails(
                vaga,
                score,
                percent,
                mustMatched,
                mustMissing,
                niceMatched,
                niceMissing
            );

            if (best is null) best = details;
            else
            {
                if (details.Score > best.Score) best = details;
                else if (details.Score == best.Score)
                {
                    if (details.MustMatched.Count > best.MustMatched.Count) best = details;
                    else if (details.MustMatched.Count == best.MustMatched.Count)
                    {
                        if (details.NiceMatched.Count > best.NiceMatched.Count) best = details;
                    }
                }
            }
        }

        return best;
    }
}
