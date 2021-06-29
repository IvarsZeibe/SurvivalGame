using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SurvivalGame
{
    public class InheritanceFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsValueType || typeToConvert.Assembly != typeof(Entity).Assembly)
            {
                return false;
            }
            else return true;
        }

        public override JsonConverter CreateConverter(
            Type type,
            JsonSerializerOptions options)
        {

            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(InheritanceConverterInner<>).MakeGenericType(type),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null);

            return converter;
        }
        private class InheritanceConverterInner<T> : JsonConverter<T>
        {
            ReferenceResolver GetReferenceResolver(JsonSerializerOptions options)
            {
                return (options.ReferenceHandler as MyReferenceHandler)._rootedResolver;
            }
            bool CheckForProp(ref Utf8JsonReader reader, string name)
            {
                var readerClone = reader;

                readerClone.Read();
                if (readerClone.TokenType != JsonTokenType.PropertyName)
                {
                    return false;
                }
                string propertyName = readerClone.GetString();
                if (propertyName == name)
                {
                    reader.Read();
                    reader.Read();
                    return true;
                }
                return false;
            }
            public InheritanceConverterInner(JsonSerializerOptions options) { }
            public override T Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }
                string id = "none";
                if (CheckForProp(ref reader, "$id"))
                    id = reader.GetString();
                string reff = "none";
                if (CheckForProp(ref reader, "$ref"))
                    reff = reader.GetString();
                CheckForProp(ref reader, "$Type");
                var asm = typeof(Entity).Assembly;
                Type type = asm.GetType(reader.GetString());
                object obj;
                if (reff == "none")
                {
                    obj = Activator.CreateInstance(type, true);
                    foreach (var prop in type.GetProperties())
                    {
                        //if (prop.CustomAttributes.Count() > 0)
                        if (prop.CustomAttributes.Any(x =>
                        {
                            if (x.AttributeType == typeof(JsonIgnoreAttribute))
                                return true;
                            else
                                return false;
                        }))
                            continue;
                        reader.Read();
                        var value = JsonSerializer.Deserialize(ref reader, prop.PropertyType, options);
                        if (prop.CanWrite)
                        {
                            prop.SetValue(obj, value);
                        }
                        reader.Read();
                    }
                }
                else
                {
                    obj = GetReferenceResolver(options).ResolveReference(reff);
                    while (reader.TokenType != JsonTokenType.EndObject)
                        reader.Read();
                }
                if (id != "none")
                    GetReferenceResolver(options).AddReference(id, obj);
                return (T)obj;
            }

            public override void Write(
                Utf8JsonWriter writer,
                T value,
                JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                var id = GetReferenceResolver(options).GetReference(value, out bool alreadyExists);
                if (!alreadyExists)
                {
                    writer.WriteString("$id", id);
                    Type type = value.GetType();
                    writer.WriteString("$Type", type.ToString());
                    foreach (var prop in type.GetProperties())
                    {
                        if (prop.CustomAttributes.Any(x =>
                        {
                            if (x.AttributeType == typeof(JsonIgnoreAttribute))
                                return true;
                            else
                                return false;
                        }))
                            continue;
                        writer.WritePropertyName(prop.Name);

                        var propValue = prop.GetValue(value);

                        JsonSerializer.Serialize(writer, propValue, options);
                    }
                }
                else
                {
                    writer.WriteString("$ref", id);
                }
                writer.WriteEndObject();
            }
        }
    }

    public class TupleIntIntConverter : JsonConverter<(int, int)>
    {
        public override (int, int) Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
            string propertyName = reader.GetString();
            if (propertyName != "TupleIntInt")
            {
                throw new JsonException();
            }
            reader.Read();
            var values = reader.GetString().Split(' ');
            (int, int) tuple = (int.Parse(values[0]), int.Parse(values[1]));
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return tuple;
                }
            }
            throw new JsonException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            (int, int) value,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("TupleIntInt", value.Item1.ToString() + " " + value.Item2.ToString());
            writer.WriteEndObject();
        }
    }
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override Vector2 Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
            string propertyName = reader.GetString();
            if (propertyName != "Vector2")
            {
                throw new JsonException();
            }
            reader.Read();
            var values = reader.GetString().Split(' ');
            Vector2 vector2 = new Vector2(float.Parse(values[0]), float.Parse(values[1]));
            reader.Read();
            return vector2;
        }

        public override void Write(
            Utf8JsonWriter writer,
            Vector2 value,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Vector2", value.X.ToString() + " " + value.Y.ToString());
            writer.WriteEndObject();
        }
    }
    class MyReferenceResolver : ReferenceResolver
    {
        private uint _referenceCount;
        private readonly Dictionary<string, object> _referenceIdToObjectMap = new();
        private readonly Dictionary<object, string> _objectToReferenceIdMap = new(ReferenceEqualityComparer.Instance);

        public override void AddReference(string referenceId, object value)
        {
            if (!_referenceIdToObjectMap.TryAdd(referenceId, value))
            {
                throw new JsonException();
            }
        }

        public override string GetReference(object value, out bool alreadyExists)
        {
            if (_objectToReferenceIdMap.TryGetValue(value, out string referenceId))
            {
                alreadyExists = true;
            }
            else
            {
                _referenceCount++;
                referenceId = _referenceCount.ToString();
                _objectToReferenceIdMap.Add(value, referenceId);
                alreadyExists = false;
            }

            return referenceId;
        }

        public override object ResolveReference(string referenceId)
        {
            if (!_referenceIdToObjectMap.TryGetValue(referenceId, out object value))
            {
                throw new JsonException();
            }

            return value;
        }
    }
    class MyReferenceHandler : ReferenceHandler
    {
        public MyReferenceHandler() => Reset();
        public ReferenceResolver _rootedResolver;
        public override ReferenceResolver CreateResolver() => _rootedResolver;
        public void Reset() => _rootedResolver = new MyReferenceResolver();
    }
}
