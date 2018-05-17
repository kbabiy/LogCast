using System;
using LogCast.Context;
using Microsoft.AspNetCore.Http;

namespace LogCast.AspNetCore
{
    public class AspNetCoreContextStrategy : ContextStrategy
    {
        private readonly IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();

        public override ContextContainer<T> GetContainer<T>()
        {
            EnsureHttpContext();
            return (ContextContainer<T>)_httpContextAccessor.HttpContext.Items[Key<T>()];
        }

        public override void RemoveContainer<T>()
        {
            EnsureHttpContext();
            _httpContextAccessor.HttpContext.Items[Key<T>()] = null;
        }

        public override void StoreContainer<T>(ContextContainer<T> container)
        {
            EnsureHttpContext();
            _httpContextAccessor.HttpContext.Items[Key<T>()] = container;
        }

        private void EnsureHttpContext()
        {
            if (_httpContextAccessor.HttpContext == null)
                throw new InvalidOperationException("Could not resolve HttpContext from HttpContentAccessor");
        }
    }
}
