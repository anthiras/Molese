using System.Text.Json;
using Framework;

namespace Test.Framework;

public class IdTests
{
    [Fact]
    public void IdHasValueEquality()
    {
        var id1 = new Id<IdTests>("foo");
        var id2 = new Id<IdTests>("foo");
        
        Assert.Equal(id1, id2);
    }

    [Fact]
    public void NewIdIsUnique()
    {
        var id1 = Id<IdTests>.New();
        var id2 = Id<IdTests>.New();
        
        Assert.NotEqual(id1, id2);
    }

    [Fact]
    public void ToStringReturnsIdValue()
    {
        var id = new Id<IdTests>("foo");
        
        Assert.Equal("foo", id.ToString());
    }

    [Fact]
    public void JsonValueIsString()
    {
        var json = JsonSerializer.Serialize(new Id<IdTests>("foo"));
        
        Assert.Equal("\"foo\"", json);
    }
}