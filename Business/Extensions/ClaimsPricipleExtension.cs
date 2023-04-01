using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPricipleExtension
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value!;
        }

        public static long GetUserId(this ClaimsPrincipal user)
        {
            return long.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        }
    }
}
