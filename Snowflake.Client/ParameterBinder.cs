using Snowflake.Client.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Snowflake.Client
{
    // Bindings: https://docs.snowflake.com/en/user-guide/python-connector-api.html
    // Based on https://github.com/snowflakedb/snowflake-connector-net/blob/master/Snowflake.Data/Core/SFDataConverter.cs

    internal static class ParameterBinder
    {
        internal static Dictionary<string, ParamBinding> BuildParameterBindings(object param)
        {
            if (param == null)
                return null;

            var paramType = param.GetType();

            if (IsSimpleType(paramType))
            {
                var bindings = new Dictionary<string, ParamBinding>();
                var binding = BuildParamFromSimpleType(param, paramType);
                bindings.Add("1", binding);
                return bindings;
            }

            if (param is IEnumerable enumerable)
            {
                return BuildParamsFromEnumerable(paramType, enumerable);
            }

            return BuildParamsFromComplexType(param, paramType);
        }

        private static Dictionary<string, ParamBinding> BuildParamsFromEnumerable(Type paramType, IEnumerable enumerable)
        {
            var result = new Dictionary<string, ParamBinding>();

            if (IsDictionary(enumerable))
            {
                if (!(enumerable is IEnumerable<KeyValuePair<string, object>> dictionary))
                {
                    throw new ArgumentException("Only IEnumerable<KeyValuePair<string, object> is supported");
                }

                foreach (var item in dictionary)
                {
                    var type = item.Value.GetType();
                    if (IsSimpleType(type))
                    {
                        result.Add(item.Key, BuildParamFromSimpleType(item.Value, type));
                    }
                    else
                    {
                        throw new ArgumentException($"Parameter binding doesn't support type {type.Name} in IEnumerable<KeyValuePair<string, object>> values.");
                    }
                }

                return result;
            }

            var elementType = GetItemTypeFromCollection(paramType);
            if (IsSimpleType(elementType))
            {
                var i = 0;
                foreach (var item in enumerable)
                {
                    i++;
                    result.Add(i.ToString(), BuildParamFromSimpleType(item, elementType));
                }

                return result;
            }

            throw new ArgumentException($"Parameter binding doesn't support type {elementType.Name} in collections.");
        }

        private static Type GetItemTypeFromCollection(Type type)
        {
            var elementType = type.GetGenericArguments().FirstOrDefault()
                                ?? type.GetElementType()
                                ?? type.GetInterfaces().FirstOrDefault(t => t.IsGenericType
                                    && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))?.GenericTypeArguments.FirstOrDefault();

            return elementType;
        }

        private static bool IsSimpleType(Type paramType)
        {
            var underlyingType = Nullable.GetUnderlyingType(paramType);
            if (underlyingType != null)
                paramType = underlyingType;

            return paramType == typeof(string) || !paramType.IsClass && !IsCustomValueType(paramType) || paramType == typeof(byte[]);
        }

        private static bool IsDictionary(object o)
        {
            if (o == null) return false;
            return o is IDictionary &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }

        private static bool IsCustomValueType(Type type)
        {
            return type.IsValueType && !type.IsPrimitive && type.Namespace != null && !type.Namespace.StartsWith("System");
        }

        private static Dictionary<string, ParamBinding> BuildParamsFromComplexType(object param, Type paramType)
        {
            var result = new Dictionary<string, ParamBinding>();

            var typeProperties = paramType.GetProperties().Where(p => IsSimpleType(p.PropertyType)).ToList();
            foreach (var property in typeProperties)
            {
                var propValue = property.GetValue(param);
                var binding = BuildParamFromSimpleType(propValue, property.PropertyType);
                result.Add(property.Name, binding);
            }

            var typeFields = paramType.GetFields().Where(p => IsSimpleType(p.FieldType)).ToList();
            foreach (var field in typeFields)
            {
                var propValue = field.GetValue(param);
                var binding = BuildParamFromSimpleType(propValue, field.FieldType);
                result.Add(field.Name, binding);
            }

            return result;
        }

        private static ParamBinding BuildParamFromSimpleType(object param, Type paramType)
        {
            var underlyingType = Nullable.GetUnderlyingType(paramType);
            if (underlyingType != null)
                paramType = underlyingType;

            var stringValue = param == null ? null : string.Format(CultureInfo.InvariantCulture, "{0}", param);

            if (TextTypes.Contains(paramType))
                return new ParamBinding() { Type = "TEXT", Value = stringValue };

            if (FixedTypes.Contains(paramType))
                return new ParamBinding() { Type = "FIXED", Value = stringValue };

            if (paramType == typeof(bool))
                return new ParamBinding() { Type = "BOOLEAN", Value = stringValue };

            if (RealTypes.Contains(paramType))
                return new ParamBinding() { Type = "REAL", Value = stringValue };

            if (paramType == typeof(DateTime))
                return new ParamBinding()
                {
                    Type = "TIMESTAMP_NTZ",
                    Value = param == null ? null : SnowflakeTypesConverter.ConvertToTimestampNtz((DateTime)param)
                };

            if (paramType == typeof(DateTimeOffset))
                return new ParamBinding()
                {
                    Type = "TIMESTAMP_TZ",
                    Value = param == null ? null : SnowflakeTypesConverter.ConvertToTimestampTz((DateTimeOffset)param)
                };

            if (paramType == typeof(byte[]))
                return new ParamBinding()
                {
                    Type = "BINARY",
                    Value = param == null ? null : SnowflakeTypesConverter.BytesToHex((byte[])param)
                };

            return null;
        }

        private static readonly HashSet<Type> FixedTypes = new HashSet<Type>()
        {
                typeof(int), typeof(long), typeof(short), typeof(sbyte),
                typeof(byte), typeof(ulong), typeof(ushort), typeof(uint)
        };

        private static readonly HashSet<Type> TextTypes = new HashSet<Type>() { typeof(string), typeof(Guid) };

        private static readonly HashSet<Type> RealTypes = new HashSet<Type>() { typeof(double), typeof(float), typeof(decimal) };
    }
}