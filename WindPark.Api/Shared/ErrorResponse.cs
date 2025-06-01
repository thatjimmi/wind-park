namespace WindPark.Shared;

public record ErrorResponse
{
    public required string Message { get; init; }
    public required IReadOnlyList<string> Errors { get; init; }

    public static ErrorResponse Create(string message, IReadOnlyList<string>? errors = null) =>
        new()
        {
            Message = message,
            Errors = errors ?? []
        };
}