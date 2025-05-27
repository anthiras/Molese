using System.Reflection;

namespace SampleApp.Api;

public class Assemblies
{
    public static readonly Assembly Domain = typeof(Domain.Aircrafts.Aircraft).Assembly;

    public static readonly Assembly Application = typeof(Application.Commands.AircraftCommandHandler).Assembly;
}