using Autofac;
using FleetManager.Shared.Interfaces;

namespace FleetManager.Shared.Core
{
    public class ServiceFactory : IServiceFactory
    {
       public T GetService<T>() where T : class, IService
        {
          return ObjectBase._container == null ? default(T) : ObjectBase._container.Resolve<T>();
        }

    }
}