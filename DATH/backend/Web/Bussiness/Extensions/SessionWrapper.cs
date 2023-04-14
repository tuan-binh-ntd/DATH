using Microsoft.AspNetCore.Http;

namespace Bussiness.Extensions
{
    public class SessionWrapper : ISession
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SessionWrapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Id => _httpContextAccessor.HttpContext!.Session.Id;

        public bool IsAvailable => _httpContextAccessor.HttpContext!.Session.IsAvailable;

        public IEnumerable<string> Keys => _httpContextAccessor.HttpContext!.Session.Keys;

        public void Clear()
        {
            _httpContextAccessor.HttpContext!.Session.Clear();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return _httpContextAccessor.HttpContext!.Session.CommitAsync(cancellationToken);
        }

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            return _httpContextAccessor.HttpContext!.Session.LoadAsync(cancellationToken);
        }

        public void Remove(string key)
        {
            _httpContextAccessor.HttpContext!.Session.Remove(key);
        }

        public void Set(string key, byte[] value)
        {
            _httpContextAccessor.HttpContext!.Session.Set(key, value);
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            return _httpContextAccessor.HttpContext!.Session.TryGetValue(key, out value);
        }
    }

}
