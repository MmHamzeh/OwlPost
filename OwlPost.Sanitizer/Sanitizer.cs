using Ganss.Xss;
using OwlPost.Core.ServicesContract;

namespace OwlPost.Sanitizer;

public class Sanitizer : ISanitizer
{
    private readonly IHtmlSanitizer _sanitizer;

    public Sanitizer(IHtmlSanitizer sanitizer)
    {
        _sanitizer = sanitizer;
    }

    public string Sanitize(string input)
    {
        return _sanitizer.Sanitize(input);
    }
}
