public class EveOptions
{
    public required string ClientId { get; set; }
    public required string SecretKey { get; set; }
    public required string AuthUrl { get; set; }
    public required string FetchTokenUrl { get; set; }
    public required string CallbackUrl { get; set; }
    public required string State { get; set; }
    public Profile? Profile { get; set; }
    public required List<EnabledScope> EnabledScopes { get; set; }
}

public class EnabledScope
{
    public required string Name { get; set; }
    public required string Address { get; set; }
}

public class Profile
{
    public required string Code { get; set; }
}