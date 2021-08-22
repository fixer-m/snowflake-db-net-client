using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snowflake.Client.Tests.UnitTests
{
    [TestFixture]
    public class ParameterBindingsMultipleValuesTest
    {
        private static IEnumerable<string> GetStringValues()
        {
            yield return "one";
            yield return "two";
            yield return "three";
        }

        [Test]
        public void BuildParameters_List()
        {
            var values = GetStringValues().ToList();

            var bindings = ParameterBinder.BuildParameterBindings(values);

            Assert.AreEqual(values.Count, bindings.Count);

            int i = 1;
            foreach (var binding in bindings)
            {
                Assert.IsTrue(binding.Key == i.ToString());
                Assert.IsTrue(binding.Value.Type == "TEXT");
                Assert.IsTrue(binding.Value.Value == values[i - 1]);

                i++;
            }
        }

        [Test]
        public void BuildParameters_Array()
        {
            var values = GetStringValues().ToArray();

            var bindings = ParameterBinder.BuildParameterBindings(values);

            Assert.AreEqual(values.Length, bindings.Count);

            int i = 1;
            foreach (var binding in bindings)
            {
                Assert.IsTrue(binding.Key == i.ToString());
                Assert.IsTrue(binding.Value.Type == "TEXT");
                Assert.IsTrue(binding.Value.Value == values[i - 1]);

                i++;
            }
        }

        [Test]
        public void BuildParameters_Enumerable()
        {
            var values = GetStringValues();
            var valuesList = values.ToList();

            var bindings = ParameterBinder.BuildParameterBindings(values);

            Assert.AreEqual(values.Count(), bindings.Count);

            int i = 1;
            foreach (var binding in bindings)
            {
                Assert.IsTrue(binding.Key == i.ToString());
                Assert.IsTrue(binding.Value.Type == "TEXT");
                Assert.IsTrue(binding.Value.Value == valuesList[i - 1]);

                i++;
            }
        }

        [Test]
        public void BuildParameters_Dictionary_Ints()
        {
            var values = new Dictionary<string, object>
            {
                { "First", 1 },
                { "Second", 2 }
            };

            var bindings = ParameterBinder.BuildParameterBindings(values);

            Assert.AreEqual(values.Count(), bindings.Count);

            int i = 0;
            foreach (var binding in bindings)
            {
                Assert.IsTrue(binding.Key == values.Keys.ElementAt(i));
                Assert.IsTrue(binding.Value.Type == "FIXED");
                Assert.IsTrue(binding.Value.Value == values.Values.ElementAt(i).ToString());

                i++;
            }
        }

        [Test]
        public void BuildParameters_Dictionary_DifferentTypes()
        {
            var values = new Dictionary<string, object>
            {
                { "1", "Sometext" },
                { "2", true },
                { "3", 777 },
                { "4", 26.5F },
                { "5", 19.239834M},
                { "6", Guid.Parse("e7412bbf-88ee-4149-b341-101e0f72ec7c") },
                { "7", new byte[] { 0, 128, 255 } }
            };

            var bindings = ParameterBinder.BuildParameterBindings(values);

            Assert.AreEqual(values.Count(), bindings.Count);


            for (int i = 0; i < bindings.Count; i++)
            {
                Assert.IsTrue(bindings.Keys.ElementAt(i) == values.Keys.ElementAt(i));
            }
        }

        [Test]
        public void BuildParameters_Dictionary_ComplexType()
        {
            var values = new Dictionary<string, object>
            {
                { "1", new CustomClass() { Property = "Str" } }
            };

            Assert.Throws<ArgumentException>(() => ParameterBinder.BuildParameterBindings(values));
        }

        [Test]
        public void BuildParameters_Dictionary_CustomClass()
        {
            var values = new Dictionary<string, CustomClass>
            {
                { "1", new CustomClass() { Property = "Str" } }
            };

            Assert.Throws<ArgumentException>(() => ParameterBinder.BuildParameterBindings(values));
        }

        [Test]
        public void BuildParameters_ListOfComplexTypes()
        {
            var values = GetCustomClassCollection().ToList();

            Assert.Throws<ArgumentException>(() => ParameterBinder.BuildParameterBindings(values));
        }

        [Test]
        public void BuildParameters_IEnumerableOfComplexTypes()
        {
            var values = GetCustomClassCollection();

            Assert.Throws<ArgumentException>(() => ParameterBinder.BuildParameterBindings(values));
        }

        private static IEnumerable<CustomClass> GetCustomClassCollection()
        {
            yield return new CustomClass() { Property = "one" };
            yield return new CustomClass() { Property = "two" };
        }

        private class CustomClass
        {
            public string Property { get; set; }
        }
    }


}
