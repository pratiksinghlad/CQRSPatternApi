using System.Text.Json;
using System.Text.Json.Serialization;

namespace CQRSPattern.Api.Features.Employee.Patch;

/// <summary>
/// Represents an optional value that can distinguish between null and not provided.
/// Supports C# 13 modern patterns and JSON serialization.
/// </summary>
/// <typeparam name="T">The type of the optional value</typeparam>
[JsonConverter(typeof(OptionalJsonConverterFactory))]
public readonly struct Optional<T> : IEquatable<Optional<T>>
{
    private readonly T? _value;
    private readonly bool _hasValue;

    private Optional(T? value, bool hasValue)
    {
        _value = value;
        _hasValue = hasValue;
    }

    /// <summary>
    /// Gets a value indicating whether this optional has a value.
    /// </summary>
    public bool HasValue => _hasValue;

    /// <summary>
    /// Gets the value if present.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no value is present</exception>
    public T? Value => _hasValue ? _value : throw new InvalidOperationException("Optional has no value");

    /// <summary>
    /// Creates an optional with no value.
    /// </summary>
    public static Optional<T> None => new(default, false);

    /// <summary>
    /// Creates an optional with a value (including null).
    /// </summary>
    /// <param name="value">The value to wrap</param>
    /// <returns>An optional containing the value</returns>
    public static Optional<T> Some(T? value) => new(value, true);

    /// <summary>
    /// Gets the value if present, otherwise returns the specified default value.
    /// </summary>
    /// <param name="defaultValue">The default value to return if no value is present</param>
    /// <returns>The value or default value</returns>
    public T? GetValueOrDefault(T? defaultValue = default) => 
        _hasValue ? _value : defaultValue;

    /// <summary>
    /// Maps the optional value to another type if present.
    /// </summary>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="mapper">The mapping function</param>
    /// <returns>A new optional with the mapped value</returns>
    public Optional<TResult> Map<TResult>(Func<T?, TResult?> mapper) =>
        _hasValue ? Optional<TResult>.Some(mapper(_value)) : Optional<TResult>.None;

    public static implicit operator Optional<T>(T? value) => Some(value);
    
    public bool Equals(Optional<T> other) => 
        _hasValue == other._hasValue && 
        (!_hasValue || EqualityComparer<T>.Default.Equals(_value, other._value));

    public override bool Equals(object? obj) => obj is Optional<T> other && Equals(other);

    public override int GetHashCode() => 
        _hasValue ? HashCode.Combine(_hasValue, _value) : _hasValue.GetHashCode();

    public static bool operator ==(Optional<T> left, Optional<T> right) => left.Equals(right);
    public static bool operator !=(Optional<T> left, Optional<T> right) => !left.Equals(right);

    public override string ToString() => 
        _hasValue ? $"Some({_value})" : "None";
}

/// <summary>
/// JSON converter factory for Optional<T> types.
/// </summary>
public class OptionalJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && 
        typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = typeToConvert.GetGenericArguments()[0];
        return (JsonConverter)Activator.CreateInstance(
            typeof(OptionalJsonConverter<>).MakeGenericType(valueType))!;
    }
}

/// <summary>
/// JSON converter for Optional<T> that handles proper serialization/deserialization.
/// </summary>
/// <typeparam name="T">The optional value type</typeparam>
public class OptionalJsonConverter<T> : JsonConverter<Optional<T>>
{
    public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return Optional<T>.Some(default);
        }

        var value = JsonSerializer.Deserialize<T>(ref reader, options);
        return Optional<T>.Some(value);
    }

    public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            JsonSerializer.Serialize(writer, value.Value, options);
        }
        else
        {
            // Don't write anything for fields that weren't provided
        }
    }

    public override bool HandleNull => true;
}
