﻿namespace SoundForest.Clients.Spotify.Authentication.Infrastructure.Responses;
internal sealed record TokenResponse
{
    public string? access_token { get; set; }
    public string? token_type { get; set; }
    public int? expires_in { get; set; }
}
