using Assessments.Transformation.DynamicProperties;
using Databank.Domain.Taxonomy;

namespace Assessments.Tests.Transformation;

public class DynamicPropertyObjectComparerTests
{
    [Fact]
    public void GetHashCode_SameObject_ReturnsSameHashCode()
    {
        // Arrange
        var comparer = new DynamicPropertyObjectComparer();
        var dynamicProperty = new DynamicProperty { Id = "1", References = new string[0], Properties = new DynamicProperty.Property[0] };

        // Act
        int hashCode1 = comparer.GetHashCode(dynamicProperty);
        int hashCode2 = comparer.GetHashCode(dynamicProperty);

        // Assert
        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void GetHashCode_EqualObjects_ReturnsSameHashCode()
    {
        // Arrange
        var comparer = new DynamicPropertyObjectComparer();
        var dynamicProperty1 = new DynamicProperty { Id = "1", References = new string[1] { "Ref1" }, Properties = new DynamicProperty.Property[1] { new DynamicProperty.Property() { Name = "test1" } } };
        var dynamicProperty2 = new DynamicProperty { Id = "1", References = new string[1] { "Ref1" }, Properties = new DynamicProperty.Property[1] { new DynamicProperty.Property() { Name = "test1" } } };

        // Act
        int hashCode1 = comparer.GetHashCode(dynamicProperty1);
        int hashCode2 = comparer.GetHashCode(dynamicProperty2);

        // Assert
        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void GetHashCode_DifferentObjects_ReturnsDifferentHashCode()
    {
        // Arrange
        var comparer = new DynamicPropertyObjectComparer();
        var dynamicProperty1 = new DynamicProperty { Id = "1", References = new string[1] { "Ref1" }, Properties = new DynamicProperty.Property[1] { new DynamicProperty.Property() { Name = "test1" } } };
        var dynamicProperty2 = new DynamicProperty { Id = "2", References = new string[1] { "Ref2" }, Properties = new DynamicProperty.Property[1] { new DynamicProperty.Property() { Name = "test2" } } };

        // Act
        int hashCode1 = comparer.GetHashCode(dynamicProperty1);
        int hashCode2 = comparer.GetHashCode(dynamicProperty2);

        // Assert
        Assert.NotEqual(hashCode1, hashCode2);
    }

    [Fact]
    public void Equals_WhenCalledWithEqualObjects_ReturnsTrue()
    {
        // Arrange
        var comparer = new DynamicPropertyObjectComparer();
        var obj1 = new DynamicProperty { Id = "1", References = new[] { "ref1", "ref2" }, Properties = new[] { new DynamicProperty.Property { Name = "prop1", Value = "value1" } } };
        var obj2 = new DynamicProperty { Id = "1", References = new[] { "ref1", "ref2" }, Properties = new[] { new DynamicProperty.Property { Name = "prop1", Value = "value1" } } };

        // Act
        var result = comparer.Equals(obj1, obj2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_WhenCalledWithDifferentObjects_ReturnsFalse()
    {
        // Arrange
        var comparer = new DynamicPropertyObjectComparer();
        var obj1 = new DynamicProperty { Id = "1", References = new[] { "ref1", "ref2" }, Properties = new[] { new DynamicProperty.Property { Name = "prop1", Value = "value1" } } };
        var obj2 = new DynamicProperty { Id = "2", References = new[] { "ref1", "ref2" }, Properties = new[] { new DynamicProperty.Property { Name = "prop1", Value = "value1" } } };

        // Act
        var result = comparer.Equals(obj1, obj2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WhenCalledWithDifferentObjectsWhereProperyDiffersTwoLevelsDown_ReturnsFalse()
    {
        // Arrange
        var comparer = new DynamicPropertyObjectComparer();
        var obj1 = new DynamicProperty { Id = "1", References = new[] { "ref1", "ref2" }, Properties = new[] { new DynamicProperty.Property { Name = "prop1", Value = "value1", Properties = new[] { new DynamicProperty.Property { Name = "prop1", Value="1" } } } } };
        var obj2 = new DynamicProperty { Id = "1", References = new[] { "ref1", "ref2" }, Properties = new[] { new DynamicProperty.Property { Name = "prop1", Value = "value1", Properties = new[] { new DynamicProperty.Property { Name = "prop2", Value = "2" } } } } };

        // Act
        var result = comparer.Equals(obj1, obj2);

        // Assert
        Assert.False(result);
    }
}