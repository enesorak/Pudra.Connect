using System.Text.Json;

namespace Pudra.Connect.App.Services;

public static class JwtParser
{
    public static string? GetRoleFromToken(string token)
    {
        try
        {
            var payload = token.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            // Rol bilgisi genellikle "role" veya tam şema adıyla bulunur.
            var roleClaimKey = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
            if (keyValuePairs.TryGetValue(roleClaimKey, out var role))
            {
                return role.ToString();
            }
        }
        catch { /* Hata durumunda null döner */ }
        return null;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}