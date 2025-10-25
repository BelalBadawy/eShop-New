using System.Collections.Concurrent;
using eShop.Application.Helpers;

namespace eShop.Application.Extensions
{
    public static class MapsterProtectExtensions
    {
        private static readonly ConcurrentDictionary<(Type, Type), bool> _configuredMappings = new();

        /// <summary>
        /// Scans assemblies and configures ID protection for all types with [ProtectId] attributes
        /// </summary>
        public static void ConfigureIdProtectionGlobally(this TypeAdapterConfig config, IdProtector protector, params Assembly[] assemblies)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(protector);

            if (assemblies == null || assemblies.Length == 0)
            {
                assemblies = new[] { Assembly.GetCallingAssembly() };
            }

            var typesWithProtectedIds = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Any(p => p.GetCustomAttribute<ProtectIdAttribute>() != null))
                .ToList();

            foreach (var destType in typesWithProtectedIds)
            {
                // Find potential source types (same name without Dto suffix, or with Dto suffix removed/added)
                var potentialSourceTypes = assemblies
                    .SelectMany(a => a.GetTypes())
                    .Where(t => IsLikelyMappingPair(t, destType))
                    .ToList();

                foreach (var srcType in potentialSourceTypes)
                {
                    ConfigureMappingPair(config, srcType, destType, protector);
                }
            }
        }

        /// <summary>
        /// Manually configure ID protection for specific type pairs
        /// </summary>
        public static void ConfigureIdProtection<TSource, TDestination>(
            this TypeAdapterConfig config,
            IdProtector protector)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(protector);

            ConfigureMappingPair(config, typeof(TSource), typeof(TDestination), protector);
        }

        private static void ConfigureMappingPair(TypeAdapterConfig config, Type srcType, Type destType, IdProtector protector)
        {
            var key = (srcType, destType);
            if (!_configuredMappings.TryAdd(key, true))
                return; // Already configured

            var method = typeof(MapsterProtectExtensions)
                .GetMethod(nameof(ConfigureMappingPairGeneric), BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(srcType, destType);

            method?.Invoke(null, new object[] { config, protector });
        }

        private static void ConfigureMappingPairGeneric<TSource, TDestination>(TypeAdapterConfig config, IdProtector protector)
        {
            config.ForType<TSource, TDestination>()
                .ApplyProtectIds(protector);
        }

        private static bool IsLikelyMappingPair(Type type1, Type type2)
        {
            if (type1 == type2) return false;

            var name1 = type1.Name.Replace("Dto", "").Replace("DTO", "");
            var name2 = type2.Name.Replace("Dto", "").Replace("DTO", "");

            return name1.Equals(name2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Extension method for individual type configuration
        /// </summary>
        public static TypeAdapterSetter<TSource, TDestination> ApplyProtectIds<TSource, TDestination>(
            this TypeAdapterSetter<TSource, TDestination> setter,
            IdProtector protector)
        {
            //ArgumentNullException.ThrowIfNull(setter);
            //ArgumentNullException.ThrowIfNull(protector);

            //var destProps = typeof(TDestination)
            //    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            //    .Where(p => p.GetCustomAttribute<ProtectIdAttribute>() != null);

            //foreach (var destProp in destProps)
            //{
            //    var srcProp = typeof(TSource).GetProperty(destProp.Name);
            //    if (srcProp == null) continue;

            //    // Entity → DTO (int to string)
            //    if (srcProp.PropertyType == typeof(int) && destProp.PropertyType == typeof(string))
            //    {
            //        setter.Map(destProp.Name,
            //            (TSource src) => protector.Protect((int)srcProp.GetValue(src)!));
            //    }
            //    // DTO → Entity (string to int)
            //    else if (srcProp.PropertyType == typeof(string) && destProp.PropertyType == typeof(int))
            //    {
            //        setter.Map(destProp.Name,
            //            (TSource src) => protector.Unprotect((string)srcProp.GetValue(src)!));
            //    }
            //    // Handle nullable types: int? → string
            //    else if (srcProp.PropertyType == typeof(int?) && destProp.PropertyType == typeof(string))
            //    {
            //        setter.Map(destProp.Name,
            //            (TSource src) => ((int?)srcProp.GetValue(src)).HasValue
            //                ? protector.Protect(((int?)srcProp.GetValue(src))!.Value)
            //                : null);
            //    }
            //    // Handle nullable types: string → int?
            //    else if (srcProp.PropertyType == typeof(string) && destProp.PropertyType == typeof(int?))
            //    {
            //        setter.Map(destProp.Name,
            //            (TSource src) => !string.IsNullOrEmpty((string)srcProp.GetValue(src))
            //                ? protector.Unprotect((string)srcProp.GetValue(src)!)
            //                : (int?)null);
            //    }
            //}

            return setter;
        }
    }
}
