using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MineSweeper.Models;

/// <summary>
/// Custom JSON converter for ObservableCollection of SweeperItem
/// </summary>
public class ObservableCollectionJsonConverter : JsonConverter<ObservableCollection<SweeperItem>>
{
    /// <summary>
    /// Reads and converts the JSON to an ObservableCollection of SweeperItem objects
    /// </summary>
    /// <param name="reader">The reader to get the value from</param>
    /// <param name="typeToConvert">The type to convert to</param>
    /// <param name="options">The serializer options</param>
    /// <returns>The converted ObservableCollection of SweeperItem objects</returns>
    /// <exception cref="JsonException">Thrown when the JSON is not in the expected format</exception>
    public override ObservableCollection<SweeperItem> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected start of array");
        }

        var collection = new ObservableCollection<SweeperItem>();
        var itemConverter = new SweeperItemJsonConverter();
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                return collection;
            }

            var item = itemConverter.Read(ref reader, typeof(SweeperItem), options);
            collection.Add(item);
        }

        throw new JsonException("Expected end of array");
    }

    /// <summary>
    /// Writes an ObservableCollection of SweeperItem objects as JSON
    /// </summary>
    /// <param name="writer">The writer to write to</param>
    /// <param name="value">The collection to convert</param>
    /// <param name="options">The serializer options</param>
    public override void Write(Utf8JsonWriter writer, ObservableCollection<SweeperItem> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        
        var itemConverter = new SweeperItemJsonConverter();
        foreach (var item in value)
        {
            itemConverter.Write(writer, item, options);
        }
        
        writer.WriteEndArray();
    }
}
