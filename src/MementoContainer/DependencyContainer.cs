using System;
using System.Collections.Generic;
using System.Linq;

namespace MementoContainer
{
    /// <summary>
    /// Simple Dependency Container
    /// </summary>
    public static class DependencyContainer
    {
        static Dictionary<Type, Func<object>> _registrations = new Dictionary<Type, Func<object>>();

        /// <summary>
        /// Set an implementation type to use as default for a given service type
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TImpl">Service implementation type</typeparam>
        public static void Register<TService, TImpl>() 
            where TImpl : TService, new()
            where TService : class
        {
            _registrations.Add(typeof(TService), () => New<TImpl>.Instance());
        }

        /// <summary>
        /// Set an implementation type to use as default for a given service type
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="instanceCreator">Function returning a service implementation instance</param>
        public static void Register<TService>(Func<TService> instanceCreator) where TService : class
        {
            _registrations.Add(typeof(TService), () => instanceCreator());
        }

        /// <summary>
        /// Set an implementation type to use as default for a given service type
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="instance">Service implementation instance</param>
        public static void Register<TService>(TService instance) where TService : class
        {
            _registrations.Add(typeof(TService), () => instance);
        }

        /// <summary>
        /// Returns the registered instance for a given service type. Throws an InvalidOperationException if no instances registered.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Registered instance for a given service type</returns>
        public static TService GetInstanceFor<TService>() where TService : class
        {
            var instance = TryGetInstanceFor<TService>();

            if(instance == null)
                throw new InvalidOperationException("No registration for " + typeof(TService));

            return instance;
        }

        /// <summary>
        /// Returns the registered instance for a given service type. Returns null if no instances registered.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Registered instance for a given service type or null if no instances registered</returns>
        public static TService TryGetInstanceFor<TService>() where TService : class
        {
            Func<object> creator;

            if (_registrations.TryGetValue(typeof(TService), out creator))
                return creator() as TService;
            else
                return default(TService);
        }
    }
}
