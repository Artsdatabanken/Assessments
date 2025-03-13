using System.Text;

namespace Assessments.Shared.Extensions;

public static class StringExtensions
{
    public static string JoinAnd<T>(this IEnumerable<T> values, string separator, string lastSeparator = null)
    {
        ArgumentNullException.ThrowIfNull(values);

        ArgumentNullException.ThrowIfNull(separator);

        var sb = new StringBuilder();

        using var enumerator = values.GetEnumerator();
        if (enumerator.MoveNext())
            sb.Append(enumerator.Current);

        var objectIsSet = false;
        object obj = null;

        if (enumerator.MoveNext())
        {
            obj = enumerator.Current;
            objectIsSet = true;
        }

        while (enumerator.MoveNext())
        {
            sb.Append(separator);
            sb.Append(obj);
            obj = enumerator.Current;
            objectIsSet = true;
        }

        if (!objectIsSet) 
            return sb.ToString();
            
        sb.Append(lastSeparator ?? separator);
        sb.Append(obj);

        return sb.ToString();
    }
}