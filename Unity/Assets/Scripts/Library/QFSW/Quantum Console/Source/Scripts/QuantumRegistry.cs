using QFSW.QC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFSW.QC
{
    public static class QuantumRegistry
    {
        private static readonly Dictionary<Type, List<object>> _objectRegistry = new Dictionary<Type, List<object>>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetRegistry()
        {
            _objectRegistry.Clear();
        }

        private static bool IsNull(object x)
        {
            if (x is UnityEngine.Object u)
            {
                return !u;
            }

            return x is null;
        }

        /// <summary>Adds the object to the registry.</summary>
        /// <param name="obj">The object to add to the registry.</param>
        /// <typeparam name="T">The type of the object to add to the registry.</typeparam>
        [Command("register-object", "Adds the object to the registry to be used by commands with MonoTargetType = Registry")]
        public static void RegisterObject<T>(T obj) where T : class { RegisterObject(typeof(T), obj); }

        /// <summary>Adds the object to the registry.</summary>
        /// <param name="type">The type of the object to add to the registry.</param>
        /// <param name="obj">The object to add to the registry.</param>
        public static void RegisterObject(Type type, object obj)
        {
            if (!type.IsClass) { throw new Exception("Registry may only contain class types"); }
            lock (_objectRegistry)
            {
                if (_objectRegistry.ContainsKey(type))
                {
                    if (_objectRegistry[type].Contains(obj))
                    {
                        throw new ArgumentException($"Could not register object '{obj}' of type {type.GetDisplayName()} as it was already registered.");
                    }

                    _objectRegistry[type].Add(obj);
                }
                else
                {
                    _objectRegistry.Add(type, new List<object>() { obj });
                }
            }
        }

        /// <summary>Removes the object from the registry.</summary>
        /// <param name="obj">The object to remove from the registry.</param>
        /// <typeparam name="T">The type of the object to remove from the registry.</typeparam>
        [Command("deregister-object", "Removes the object to the registry to be used by commands with MonoTargetType = Registry")]
        public static void DeregisterObject<T>(T obj) where T : class { DeregisterObject(typeof(T), obj); }

        /// <summary>Removes the object to the registry.</summary>
        /// <param name="type">The type of the object to remove from the registry.</param>
        /// <param name="obj">The object to remove from the registry.</param>
        public static void DeregisterObject(Type type, object obj)
        {
            if (!type.IsClass) { throw new Exception("Registry may only contain class types"); }
            lock (_objectRegistry)
            {
                if (_objectRegistry.ContainsKey(type) && _objectRegistry[type].Contains(obj))
                {
                    _objectRegistry[type].Remove(obj);
                }
                else
                {
                    throw new ArgumentException($"Could not deregister object '{obj}' of type {type.GetDisplayName()} as it was not found in the registry.");
                }
            }
        }

        /// <summary>Gets the size of the specified registry.</summary>
        /// <returns>The registry size.</returns>
        /// <typeparam name="T">The registry to query.</typeparam>
        public static int GetRegistrySize<T>() where T : class { return GetRegistrySize(typeof(T)); }

        /// <summary>Gets the size of the specified registry.</summary>
        /// <returns>The registry size.</returns>
        /// <param name="type">The registry to query.</param>
        public static int GetRegistrySize(Type type)
        {
            return GetRegistryContents(type).Count();
        }

        /// <summary>Gets the contents of the specified registry.</summary>
        /// <returns>The registry contents.</returns>
        /// <typeparam name="T">The registry to query.</typeparam>
        public static IEnumerable<T> GetRegistryContents<T>() where T : class
        {
            foreach (object obj in GetRegistryContents(typeof(T)))
            {
                yield return (T)obj;
            }
        }

        /// <summary>Gets the contents of the specified registry.</summary>
        /// <returns>The registry contents.</returns>
        /// <param name="type">The registry to query.</param>
        public static IEnumerable<object> GetRegistryContents(Type type)
        {
            if (!type.IsClass) { throw new Exception("Registry may only contain class types"); }
            lock (_objectRegistry)
            {
                if (_objectRegistry.ContainsKey(type))
                {
                    List<object> registry = _objectRegistry[type];
                    registry.RemoveAll(IsNull);
                    return registry;
                }
                
                return Enumerable.Empty<object>();
            }
        }

        /// <summary>clears the contents of the specified registry.</summary>
        /// <typeparam name="T">The registry to clear.</typeparam>
        [Command("clear-registry", "Clears the contents of the specified registry")]
        public static void ClearRegistryContents<T>() where T : class
        {
            ClearRegistryContents(typeof(T));
        }

        /// <summary>clears the contents of the specified registry.</summary>
        /// <param name="type">The registry to clear.</param>
        public static void ClearRegistryContents(Type type)
        {
            if (!type.IsClass) { throw new Exception("Registry may only contain class types"); }
            lock (_objectRegistry)
            {
                if (_objectRegistry.ContainsKey(type))
                {
                    _objectRegistry[type].Clear();
                }
            }
        }

        [Command("display-registry", "Displays the contents of the specified registry")]
        private static IEnumerable<object> DisplayRegistry<T>() where T : class
        {
            if (GetRegistrySize<T>() <= 0) 
            { 
                return ($"The registry '{typeof(T).GetDisplayName()}' is empty").Yield();
            }

            return GetRegistryContents<T>();
        }
    }
}