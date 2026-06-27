using System.Reflection;
using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Entities.Players;

public interface ITopBarElement
{
    /// <summary>res:// path to the PackedScene for this element.</summary>
    string ScenePath { get; }

    /// <summary>Whether this element should appear for the given player.</summary>
    Func<Player, bool> CanUse { get; }

    /// <summary>
    ///     Width reserved in the top bar spacer for this element.
    ///     Typically matches the horizontal gap between elements (e.g. 80f).
    /// </summary>
    float Width { get; }

    /// <summary>Called after the element is added to the scene.</summary>
    void Initialize(Player player);
}

// ── Registry ──────────────────────────────────────────────────────────────────

internal static class TopBarElementRegistry
{
    private static List<Type>? _types;

    internal static IReadOnlyList<Type> Types => _types ??= Discover();

    private static List<Type> Discover()
    {
        var results = new List<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            IEnumerable<Type> types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null)!;
            }

            results.AddRange(types.Where(t =>
                t is { IsClass: true, IsAbstract: false } &&
                t.IsAssignableTo(typeof(ITopBarElement))));
        }

        return results;
    }
    
    internal static ITopBarElement CreateInstance(Type type)
    {
        return (ITopBarElement)Activator.CreateInstance(type)!;
    }

    internal static (string scenePath, Func<Player, bool> canUse, float width) ReadMetadata(Type type)
    {
        var probe = (ITopBarElement)RuntimeHelpers.GetUninitializedObject(type);
        return (probe.ScenePath, probe.CanUse, probe.Width);
    }
}