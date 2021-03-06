﻿using NUnit.Framework;
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
            yield return new CustomClass() { Property = "one" } ;
            yield return new CustomClass() { Property = "two" };
        }

        private class CustomClass
        {
            public string Property { get; set; }
        }
    }


}
