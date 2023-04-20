namespace IdentityProvider.Helper;

public record AuthCode(
    string ClientId,
    string CodeChallenge,
    string CodeChallengeMethod,
    string RedirectUri,
    DateTime Expiracy
    );
