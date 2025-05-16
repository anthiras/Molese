using System.Reflection;
using AutoFixture;
using AutoFixture.Xunit2;

namespace Test.TestUtils;

public class InlineCustomizationAttribute(Action<IFixture> customize) : CustomizeAttribute
{
    public override ICustomization GetCustomization(ParameterInfo parameter)
    {
        return new InlineCustomization(customize);
    }
}

public class InlineCustomization(Action<IFixture> customize) : ICustomization
{
    public void Customize(IFixture fixture)
    {
        customize(fixture);
    }
}