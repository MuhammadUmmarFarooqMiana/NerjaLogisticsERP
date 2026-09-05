namespace NerjaLogisticsERP.Application.Common.Exceptions;

public class IdentityException : Exception
{
    public IdentityException(IEnumerable<string> errors)
        : base(string.Join("; ", errors))
    {
        Errors = errors.ToArray();
    }

    public string[] Errors { get; }
}
