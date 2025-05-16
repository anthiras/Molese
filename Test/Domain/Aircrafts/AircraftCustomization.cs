using AutoFixture;
using Domain.Aircrafts;
using Framework;
using Test.TestUtils;

namespace Test.Domain.Aircrafts;

public class AircraftCustomization() : InlineCustomization(fixture =>
    fixture.Register((Id<Aircraft> id, AircraftRegistration reg) =>
        new Aircraft(id, reg)));

public class AssignedAttribute() : InlineCustomizationAttribute(fixture => 
    fixture.Register((Id<Aircraft> id, AircraftRegistration reg, Assignment assignment) =>
    {
        var aircraft = new Aircraft(id, reg);
        aircraft.Assign(assignment);
        return aircraft;
    }));