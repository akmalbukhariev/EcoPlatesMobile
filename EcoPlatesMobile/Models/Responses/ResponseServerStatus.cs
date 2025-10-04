public sealed class ResponseServerStatus
{
    // matches Actuator JSON: {"status":"UP"}
    [Newtonsoft.Json.JsonProperty("status")]
    public string? Status { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public bool IsUp => string.Equals(Status, "UP", StringComparison.OrdinalIgnoreCase);

    [Newtonsoft.Json.JsonIgnore]
    public int LatencyMs { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public DateTimeOffset CheckedAt { get; set; }

    // Optional: keep the raw body for debugging
    [Newtonsoft.Json.JsonIgnore]
    public string? RawJson { get; set; }
}