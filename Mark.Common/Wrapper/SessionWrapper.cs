using System;
using Microsoft.AspNetCore.Http;
using Mark.Common.ExtensionMethod;
using Mark.Common.Helper;

namespace Mark.Common.Wrapper
{
    public interface ISessionWapper<T>
    {
        T Element { get; set; }
    }

    public class SessionWapper<T> : ISessionWapper<T>
    {
        private static readonly string _userKey = $"session.{nameof(T)}";
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SessionWapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public T Element
        {
            get => Session.GetObject<T>(_userKey);
            set => Session.SetObject(_userKey, value);
        }

        public bool Remove()
        {
            var res = true;
            try
            {
                Session.Remove(_userKey);
            }
            catch (Exception e)
            {
                res = false;
                Logger.Error(e.Message);
            }
            return res;
        }
    }
}
