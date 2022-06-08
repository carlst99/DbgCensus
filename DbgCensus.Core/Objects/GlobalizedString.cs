using System.Text.Json.Serialization;

namespace DbgCensus.Core.Objects;

/// <summary>
/// Represents a globalized string model.
/// </summary>
/// <param name="English">The English translation of the string.</param>
/// <param name="German">The German translation of the string.</param>
/// <param name="French">The French translation of the string.</param>
/// <param name="Italian">The Italian translation of the string.</param>
/// <param name="Spanish">The Spanish translation of the string.</param>
/// <param name="Turkish">The Turkish translation of the string.</param>
public record GlobalizedString
(
    [property: JsonPropertyName("en")]
    Optional<string> English,
    [property: JsonPropertyName("de")]
    Optional<string> German,
    [property: JsonPropertyName("fr")]
    Optional<string> French,
    [property: JsonPropertyName("it")]
    Optional<string> Italian,
    [property: JsonPropertyName("es")]
    Optional<string> Spanish,
    [property: JsonPropertyName("tr")]
    Optional<string> Turkish
);
