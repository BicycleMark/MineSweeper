using System.Text.Json;
using System.Text.Json.Serialization;

namespace MineSweeper.Models;

/// <summary>
/// Custom JSON converter for GameEnums.GameStatus
/// </summary>
public class GameStatusJsonConverter : JsonConverter<GameEnums.GameStatus>
{
    /// <summary>
    /// Reads and converts the JSON to a GameStatus enum
    /// </summary>
    /// <param name="reader">The reader to get the value from</param>
    /// <param name="typeToConvert">The type to convert to</param>
    /// <param name="options">The serializer options</param>
    /// <returns>The converted GameStatus enum value</returns>
    public override GameEnums.GameStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string enumString = reader.GetString();
            if (Enum.TryParse<GameEnums.GameStatus>(enumString, out var result))
            {
                return result;
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            int enumValue = reader.GetInt32();
            if (Enum.IsDefined(typeof(GameEnums.GameStatus), enumValue))
            {
                return (GameEnums.GameStatus)enumValue;
            }
        }
        
        return GameEnums.GameStatus.NotStarted; // Default value
    }

    /// <summary>
    /// Writes a GameStatus enum as a JSON string
    /// </summary>
    /// <param name="writer">The writer to write to</param>
    /// <param name="value">The value to convert</param>
    /// <param name="options">The serializer options</param>
    public override void Write(Utf8JsonWriter writer, GameEnums.GameStatus value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

/// <summary>
/// Custom JSON converter for GameEnums.GameDifficulty
/// </summary>
public class GameDifficultyJsonConverter : JsonConverter<GameEnums.GameDifficulty>
{
    /// <summary>
    /// Reads and converts the JSON to a GameDifficulty enum
    /// </summary>
    /// <param name="reader">The reader to get the value from</param>
    /// <param name="typeToConvert">The type to convert to</param>
    /// <param name="options">The serializer options</param>
    /// <returns>The converted GameDifficulty enum value</returns>
    public override GameEnums.GameDifficulty Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string enumString = reader.GetString();
            if (Enum.TryParse<GameEnums.GameDifficulty>(enumString, out var result))
            {
                return result;
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            int enumValue = reader.GetInt32();
            if (Enum.IsDefined(typeof(GameEnums.GameDifficulty), enumValue))
            {
                return (GameEnums.GameDifficulty)enumValue;
            }
        }
        
        return GameEnums.GameDifficulty.Easy; // Default value
    }

    /// <summary>
    /// Writes a GameDifficulty enum as a JSON string
    /// </summary>
    /// <param name="writer">The writer to write to</param>
    /// <param name="value">The value to convert</param>
    /// <param name="options">The serializer options</param>
    public override void Write(Utf8JsonWriter writer, GameEnums.GameDifficulty value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
