using System;
using System.Collections.Concurrent;
using System.Resources;

namespace Assessments.Shared.Helpers;

public class ResourceManagerHelper
{
    private static readonly Lazy<ResourceManagerHelper> Lazy = new(() => new ResourceManagerHelper(), true);

    private readonly ConcurrentDictionary<Type, ResourceManager> _managers = new();

    private static ResourceManagerHelper Instance { get { return Lazy.Value; } }

    public static ResourceManager GetManager(Type resourceType) => Instance.GetResourceManager(resourceType);

    private ResourceManager GetResourceManager(Type resourceType)
    {
        return _managers.GetOrAdd(resourceType, ValueFactory);
        
        static ResourceManager ValueFactory(Type resourceType) => new(resourceType);
    }
}