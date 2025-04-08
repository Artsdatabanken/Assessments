using Assessments.Transformation.DynamicProperties;
using Databank.Domain.Taxonomy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Assessments.Tests.Transformation
{
    [TestClass]
    public class DynamicPropertyObjectComparerTests
    {
        [TestMethod]
        public void GetHashCode_SameObject_ReturnsSameHashCode()
        {
            // Arrange
            var comparer = new DynamicPropertyObjectComparer();
            var dynamicProperty = new DynamicProperty { Id = "1", References = new string[0], Properties = new DynamicProperty.Property[0] };

            // Act
            int hashCode1 = comparer.GetHashCode(dynamicProperty);
            int hashCode2 = comparer.GetHashCode(dynamicProperty);

            // Assert
            Assert.AreEqual(hashCode1, hashCode2);
        }

        [TestMethod]
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
            Assert.AreEqual(hashCode1, hashCode2);
        }

        [TestMethod]
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
            Assert.AreNotEqual(hashCode1, hashCode2);
        }

        [TestMethod]
        public void Equals_WhenCalledWithEqualObjects_ReturnsTrue()
        {
            // Arrange
            var comparer = new DynamicPropertyObjectComparer();
            var obj1 = new DynamicProperty { Id = "1", References = new[] { "ref1", "ref2" }, Properties = new[] { new DynamicProperty.Property { Name = "prop1", Value = "value1" } } };
            var obj2 = new DynamicProperty { Id = "1", References = new[] { "ref1", "ref2" }, Properties = new[] { new DynamicProperty.Property { Name = "prop1", Value = "value1" } } };

            // Act
            var result = comparer.Equals(obj1, obj2);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_WhenCalledWithDifferentObjects_ReturnsFalse()
        {
            // Arrange
            var comparer = new DynamicPropertyObjectComparer();
            var obj1 = new DynamicProperty { Id = "1", References = new[] { "ref1", "ref2" }, Properties = new[] { new DynamicProperty.Property { Name = "prop1", Value = "value1" } } };
            var obj2 = new DynamicProperty { Id = "2", References = new[] { "ref1", "ref2" }, Properties = new[] { new DynamicProperty.Property { Name = "prop1", Value = "value1" } } };

            // Act
            var result = comparer.Equals(obj1, obj2);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_WhenCalledWithDifferentObjectsWhereProperyDiffersTwoLevelsDown_ReturnsFalse()
        {
            // Arrange
            var comparer = new DynamicPropertyObjectComparer();
            var obj1 = new DynamicProperty { Id = "1", References = new[] { "ref1", "ref2" }, Properties = new[] { new DynamicProperty.Property { Name = "prop1", Value = "value1", Properties = new[] { new DynamicProperty.Property { Name = "prop1", Value="1" } } } } };
            var obj2 = new DynamicProperty { Id = "1", References = new[] { "ref1", "ref2" }, Properties = new[] { new DynamicProperty.Property { Name = "prop1", Value = "value1", Properties = new[] { new DynamicProperty.Property { Name = "prop2", Value = "2" } } } } };

            // Act
            var result = comparer.Equals(obj1, obj2);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
