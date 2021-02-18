using NUnit.Framework;

namespace Snowflake.Client.Tests.UnitTests
{
    [TestFixture]
    public class ParameterBindingsComplexTypesTest
    {
        [Test]
        public void BuildParameters_FromClass_Properties()
        {
            var value = new CustomClassWithProperties() { IntProperty = 2, StringProperty = "test" };

            var bindings = ParameterBinder.BuildParameterBindings(value);

            Assert.IsTrue(bindings.ContainsKey(nameof(value.IntProperty)));
            Assert.IsTrue(bindings[nameof(value.IntProperty)].Type == "FIXED");
            Assert.IsTrue(bindings[nameof(value.IntProperty)].Value == value.IntProperty.ToString());

            Assert.IsTrue(bindings.ContainsKey(nameof(value.StringProperty)));
            Assert.IsTrue(bindings[nameof(value.StringProperty)].Type == "TEXT");
            Assert.IsTrue(bindings[nameof(value.StringProperty)].Value == value.StringProperty);
        }

        [Test]
        public void BuildParameters_FromStruct_Properties()
        {
            var value = new CustomStructWithProperties() { IntProperty = 2, StringProperty = "test" };

            var bindings = ParameterBinder.BuildParameterBindings(value);

            Assert.IsTrue(bindings.ContainsKey(nameof(value.IntProperty)));
            Assert.IsTrue(bindings[nameof(value.IntProperty)].Type == "FIXED");
            Assert.IsTrue(bindings[nameof(value.IntProperty)].Value == value.IntProperty.ToString());

            Assert.IsTrue(bindings.ContainsKey(nameof(value.StringProperty)));
            Assert.IsTrue(bindings[nameof(value.StringProperty)].Type == "TEXT");
            Assert.IsTrue(bindings[nameof(value.StringProperty)].Value == value.StringProperty);
        }

        [Test]
        public void BuildParameters_FromClass_Fields()
        {
            var value = new CustomClassWithFields() { IntField = 2, StringField = "test" };

            var bindings = ParameterBinder.BuildParameterBindings(value);

            Assert.IsTrue(bindings.ContainsKey(nameof(value.IntField)));
            Assert.IsTrue(bindings[nameof(value.IntField)].Type == "FIXED");
            Assert.IsTrue(bindings[nameof(value.IntField)].Value == value.IntField.ToString());

            Assert.IsTrue(bindings.ContainsKey(nameof(value.StringField)));
            Assert.IsTrue(bindings[nameof(value.StringField)].Type == "TEXT");
            Assert.IsTrue(bindings[nameof(value.StringField)].Value == value.StringField);
        }

        [Test]
        public void BuildParameters_FromStruct_Fields()
        {
            var value = new CustomStructWithFields() { IntField = 2, StringField = "test" };

            var bindings = ParameterBinder.BuildParameterBindings(value);

            Assert.IsTrue(bindings.ContainsKey(nameof(value.IntField)));
            Assert.IsTrue(bindings[nameof(value.IntField)].Type == "FIXED");
            Assert.IsTrue(bindings[nameof(value.IntField)].Value == value.IntField.ToString());

            Assert.IsTrue(bindings.ContainsKey(nameof(value.StringField)));
            Assert.IsTrue(bindings[nameof(value.StringField)].Type == "TEXT");
            Assert.IsTrue(bindings[nameof(value.StringField)].Value == value.StringField);
        }

        [Test]
        public void BuildParameters_FromAnonymousType()
        {
            var value = new { IntProperty = 2, StringProperty = "test" };

            var bindings = ParameterBinder.BuildParameterBindings(value);

            Assert.IsTrue(bindings.ContainsKey(nameof(value.IntProperty)));
            Assert.IsTrue(bindings[nameof(value.IntProperty)].Type == "FIXED");
            Assert.IsTrue(bindings[nameof(value.IntProperty)].Value == value.IntProperty.ToString());

            Assert.IsTrue(bindings.ContainsKey(nameof(value.StringProperty)));
            Assert.IsTrue(bindings[nameof(value.StringProperty)].Type == "TEXT");
            Assert.IsTrue(bindings[nameof(value.StringProperty)].Value == value.StringProperty);
        }
        private class CustomClassWithProperties
        {
            public string StringProperty { get; set; }

            public int IntProperty { get; set; }
        }

        private class CustomClassWithFields
        {
            public string StringField;

            public int IntField;
        }

        private struct CustomStructWithProperties
        {
            public string StringProperty { get; set; }

            public int IntProperty { get; set; }
        }

        private struct CustomStructWithFields
        {
            public string StringField;

            public int IntField;
        }
    }
}
