namespace NerjaLogisticsERP.Application.Auth.Dtos;

public record AuthTokenResponseDto(string AccessToken, DateTimeOffset ExpiresAt);
