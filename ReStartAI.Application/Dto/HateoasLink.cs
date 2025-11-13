namespace ReStartAI.Application.Dto
{
    public class HateoasLink
    {
        public string Rel { get; set; } = string.Empty;
        public string Href { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;

        public HateoasLink(string rel, string href, string method)
        {
            Rel = rel;
            Href = href;
            Method = method;
        }
    }
}