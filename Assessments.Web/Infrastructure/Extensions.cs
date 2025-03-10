using System.Text;
using System.Web;

namespace Assessments.Web.Infrastructure
{
    public static class Extensions
    {
        public static string JoinAnd<T>(this IEnumerable<T> values, string separator, string lastSeparator = null)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            
            if (separator == null)
                throw new ArgumentNullException(nameof(separator));

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
        
        public static string AddParameters(this QueryString queryString, object routeValues)
        {
            var nameValueCollection = HttpUtility.ParseQueryString(queryString.ToString());

            var values = new RouteValueDictionary(routeValues);

            foreach (var element in values)
            {
                nameValueCollection.Remove(element.Key);
                nameValueCollection.Add(element.Key, element.Value!.ToString());
            }

            return $"?{nameValueCollection}";
        }

        public static string RemoveParameters(this QueryString queryString, IEnumerable<string> parameters)
        {
            var nameValueCollection = HttpUtility.ParseQueryString(queryString.ToString());

            foreach (var element in nameValueCollection.AllKeys.Where(parameters.Contains))
                nameValueCollection.Remove(element);

            return $"?{nameValueCollection}";
        }

        public static void Remove<T>(this IList<T> list, Type type)
        {
            var instances = list.Where(x => x.GetType() == type).ToList();
            instances.ForEach(obj => list.Remove(obj));
        }
    }
}
