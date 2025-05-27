using System.ComponentModel;
using SampleApp.Domain.Aircrafts;
using Framework;

namespace SampleApp.Application.Queries;

public record GetAllAircrafts();

public record GetAircraftById(
    [property:Description("Aircraft ID")]
    Id<Aircraft> AircraftId);