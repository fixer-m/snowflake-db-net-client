using Snowflake.Client.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Snowflake.Client
{
    // Bindings https://docs.snowflake.com/en/user-guide/python-connector-api.html
    // Based on https://github.com/snowflakedb/snowflake-connector-net/blob/master/Snowflake.Data/Core/SFDataConverter.cs
    public static class ParameterBinder
    {
        public static Dictionary<string, ParamBinding> BuildParameterBindings(object param)
        {
            if (param == null)
                return null;

            var bindings = new Dictionary<string, ParamBinding>();
            var paramType = param.GetType();

            if (IsSimpleType(paramType))
            {
                var binding = GetParamFromSimpleType(param, paramType);
                bindings.Add("1", binding);
                return bindings;
            }

            if (IsCollectionOfSimpleType(paramType))
            {
                var elementType = paramType.GetGenericArguments().FirstOrDefault() ?? paramType.GetElementType();

                var enumInterface = (IEnumerable)param;
                int i = 0;
                foreach (var item in enumInterface)
                {
                    i++;
                    bindings.Add(i.ToString(), GetParamFromSimpleType(item, elementType));
                }

                return bindings;
            }

            bindings = GetParamsFromComplexType(param, paramType);

            return bindings;
        }

        private static bool IsCollectionOfSimpleType(Type type)
        {
            if (type.GetInterface(nameof(IEnumerable)) == null)
                return false;

            var elementType = type.GetGenericArguments().FirstOrDefault() ?? type.GetElementType();

            return IsSimpleType(elementType);
        }

        private static bool IsSimpleType(Type paramType)
        {
            var underlyingType = Nullable.GetUnderlyingType(paramType);
            if (underlyingType != null)
                paramType = underlyingType;

            // Will treat struct as simple class.
            return paramType == typeof(string) || !paramType.IsClass || paramType == typeof(byte[]);
        }

        private static Dictionary<string, ParamBinding> GetParamsFromComplexType(object param, Type paramType)
        {
            var result = new Dictionary<string, ParamBinding>();
            var typeProperties = paramType.GetProperties().Where(p => IsSimpleType(p.PropertyType)).ToList();

            foreach (var property in typeProperties)
            {
                var propValue = property.GetValue(param);
                var binding = GetParamFromSimpleType(propValue, property.PropertyType);
                result.Add(property.Name, binding);
            }

            return result;
        }

        private static ParamBinding GetParamFromSimpleType(object param, Type paramType)
        {
            var underlyingType = Nullable.GetUnderlyingType(paramType);
            if (underlyingType != null)
                paramType = underlyingType;

            var stringValue = param == null ? null : String.Format(CultureInfo.InvariantCulture, "{0}", param);

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