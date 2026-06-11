using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace LitXusTravel.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public string Value { get; }

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email cannot be empty.");

        if (!EmailRegex.IsMatch(value))
            throw new DomainException($"'{value}' is not a valid email address.");

        Value = value.ToLowerInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
