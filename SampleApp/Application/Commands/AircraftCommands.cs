using System.ComponentModel;
using SampleApp.Domain.Aircrafts;
using Framework;

namespace SampleApp.Application.Commands;

public record CreateAircraft(
    [property: Description("The aircraft registration number")]
    AircraftRegistration Registration);

public record DeleteAircraft(
    [property: Description("Aircraft ID")]
    Id<Aircraft> Id);