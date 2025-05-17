using AutoFixture;
using SampleApp.Domain.Airports;

namespace SampleApp.Test.Domain.Airports;

public class AirportCodeCustomization : ICustomization
{
    private readonly Random _random = new();
    public void Customize(IFixture fixture)
    {
        fixture.Register(() =>
            new AirportCode(new string(new[] { RandomLetter(), RandomLetter(), RandomLetter() })));
    }
    
    private char RandomLetter() => (char)('A' + _random.Next(0,26));
}