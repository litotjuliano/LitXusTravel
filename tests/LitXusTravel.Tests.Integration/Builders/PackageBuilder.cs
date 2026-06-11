using LitXusTravel.Domain.Entities;

namespace LitXusTravel.Tests.Integration.Builders;

public class PackageBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _title = "Test Package";
    private string _destination = "Kuala Lumpur";
    private decimal _basePrice = 1000m;
    private int _durationDays = 5;
    private string? _category = "Beach & Resort";
    private string? _description = "A great test package";
    private string? _region = "Asia-Pacific";
    private PackageVisibility _visibility = PackageVisibility.Published;

    public PackageBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public PackageBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public PackageBuilder WithDestination(string destination)
    {
        _destination = destination;
        return this;
    }

    public PackageBuilder WithPrice(decimal price)
    {
        _basePrice = price;
        return this;
    }

    public PackageBuilder WithDuration(int days)
    {
        _durationDays = days;
        return this;
    }

    public PackageBuilder WithCategory(string category)
    {
        _category = category;
        return this;
    }

    public PackageBuilder WithVisibility(PackageVisibility visibility)
    {
        _visibility = visibility;
        return this;
    }

    public Package Build()
    {
        var package = Package.Create(
            _title, _destination, _basePrice, _durationDays,
            category: _category, description: _description);

        // Set region and visibility via UpdateDetails
        package.UpdateDetails(
            _title, _description, null, _category, _basePrice, _durationDays,
            _destination, _region, null, null, null, null, null, null, null);

        if (_visibility == PackageVisibility.Published)
            package.Publish();

        return package;
    }
}
