using System.ComponentModel;
using Domain.Aircrafts;
using Framework;

namespace Application.Queries;

public record GetAllAircrafts();

public record GetAircraftById(
    [property:Description("Aircraft ID")]
    Id<Aircraft> Id);