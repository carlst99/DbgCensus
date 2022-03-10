using DbgCensus.Core.Objects;
using System.Text.Json.Serialization;

namespace RestSample.Objects;

/// <summary>
/// Represents a Census field containing localized strings.
/// Properties are optional as the languages returned can be
/// customized with the <c>c:lang</c> command.
/// </summary>
/// <param name="English"></param>
/// <param name="German"></param>
/// <param name="French"></param>
/// <param name="Italian"></param>
/// <param name="Spanish"></param>
/// <param name="Turkish"></param>
public record LocalizedText
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
