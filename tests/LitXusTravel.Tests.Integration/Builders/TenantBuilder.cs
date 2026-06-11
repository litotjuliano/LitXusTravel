using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.ValueObjects;

namespace LitXusTravel.Tests.Integration.Builders;

public class TenantBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Test Tenant";
    private string _email = "test@example.com";
    private string _phone = "+1234567890";
    private string? _website = "https://example.com";
    private string? _subdomain = "test";

    public TenantBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public TenantBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public TenantBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public TenantBuilder WithSubdomain(string subdomain)
    {
        _subdomain = subdomain;
        return this;
    }

    public Tenant Build()
    {
        var tenant = Tenant.Create(_name, new Email(_email), _phone, _website);
        if (_subdomain != null)
            tenant.AssignSubdomain(_subdomain);
        return tenant;
    }
}
