namespace Domain.Core.Model
{
    public class RelativeLink
    {
        public string Href { get; }
        public string Rel { get; }
        public string Method { get; }
        public RelativeLink(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }

    public enum ResourceUriType
    {
        PreviousPage,
        NextPage,
        CurrentPage
    }
}
