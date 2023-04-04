using System.Security.Claims;

namespace Bussiness.Extensions
{
    public static class ClaimsPricipleExtension
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value!;
        }

        public static TPrimaryKey GetUserId<TPrimaryKey>(this ClaimsPrincipal user)
        {
            return ConvertValue<TPrimaryKey>(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        }
        public static TPrimaryKey ConvertValue<TPrimaryKey>(string value)
        {
            return (TPrimaryKey)Convert.ChangeType(value, typeof(TPrimaryKey));
        }
    }
}