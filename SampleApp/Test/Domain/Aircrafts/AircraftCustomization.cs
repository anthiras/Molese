using AutoFixture;
using Framework;
using SampleApp.Domain.Aircrafts;
using Test.TestUtils;

namespace SampleApp.Test.Domain.Aircrafts;

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