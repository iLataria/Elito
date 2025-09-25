using Bunker.Core.Services.Base;
using System;

namespace Bunker.Core.Services
{
    public abstract class CoreViewMediator : IDisposable
    {
        private readonly CoreService _model;

        public CoreViewMediator(CoreService model)
        {
            _model = model;
        }

        public virtual void Dispose()
        {
            
        }
    }
}
