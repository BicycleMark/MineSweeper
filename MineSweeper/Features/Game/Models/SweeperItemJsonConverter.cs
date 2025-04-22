using System.Text.Json.Serialization;

namespace MineSweeper.Features.Game.Models;

/// <summary>
///     Custom JSON converter for SweeperItem to handle serialization and deserialization
/// </summary>
public class SweeperItemJsonConverter : JsonConverter<SweeperItem>
{
    /// <summary>
    ///     Reads and converts the JSON to a SweeperItem object
    /// </summary>
    /// <param name="reader">The reader to get the value from</param>
    /// <param name="typeToConvert">The type to convert to</param>
    /// <param name="options">The serializer options</param>
    /// <returns>The converted SweeperItem object</returns>
    /// <exception cref="JsonException">Thrown when the JSON is not in the expected format</exception>
    public override SweeperItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of object");

        var item = new SweeperItem();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return item;

            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

            var propertyName = reader.GetString();
            if (propertyName == null) throw new JsonException("Property name cannot be null");
            reader.Read();

            switch (propertyName)
            {
                case "IsRevealed":
                    item.IsRevealed = reader.GetBoolean();
                    break;
                case "IsMine":
                    item.IsMine = reader.GetBoolean();
                    break;
                case "IsFlagged":
                    item.IsFlagged = reader.GetBoolean();
                    break;
                case "MineCount":
                    item.MineCount = reader.GetInt32();
                    break;
                case "Point":
                    if (reader.TokenType == JsonTokenType.StartObject)
                    {
                        double x = 0, y = 0;

                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndObject) break;

                            if (reader.TokenType != JsonTokenType.PropertyName)
                                throw new JsonException("Expected property name in Point object");

                            var pointProperty = reader.GetString();
                            if (pointProperty == null) throw new JsonException("Point property name cannot be null");
                            reader.Read();

                            switch (pointProperty)
                            {
                                case "X":
                                    x = reader.GetDouble();
                                    break;
                                case "Y":
                                    y = reader.GetDouble();
                                    break;
                                default:
                                    reader.Skip();
                                    break;
                            }
                        }

                        item.Point = new Point(x, y);
                    }

                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        throw new JsonException("Expected end of object");
    }

    /// <summary>
    ///     Writes a SweeperItem object as JSON
    /// </summary>
    /// <param name="writer">The writer to write to</param>
    /// <param name="value">The value to convert</param>
    /// <param name="options">The serializer options</param>
    public override void Write(Utf8JsonWriter writer, SweeperItem value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteBoolean("IsRevealed", value.IsRevealed);
        writer.WriteBoolean("IsMine", value.IsMine);
        writer.WriteBoolean("IsFlagged", value.IsFlagged);
        writer.WriteNumber("MineCount", value.MineCount);

        writer.WriteStartObject("Point");
        writer.WriteNumber("X", value.Point.X);
        writer.WriteNumber("Y", value.Point.Y);
        writer.WriteEndObject();

        writer.WriteEndObject();
    }
}
