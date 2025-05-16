using System.ComponentModel;
using Domain.Aircrafts;
using Framework;

namespace Application.Commands;

public record CreateAircraft(
    [property: Description("The aircraft registration number")]
    AircraftRegistration Registration);

public record DeleteAircraft(
    [property: Description("Aircraft ID")]
    Id<Aircraft> Id);