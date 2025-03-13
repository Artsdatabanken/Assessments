using System.Web;

namespace Assessments.Web.Infrastructure
{
    public static class Extensions
    {
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

        public static string RemoveParametersExcept(this QueryString queryString, IEnumerable<string> parameters)
        {
            var nameValueCollection = HttpUtility.ParseQueryString(queryString.ToString());

            foreach (var element in nameValueCollection.AllKeys.Except(parameters))
                nameValueCollection.Remove(element);

            return $"?{nameValueCollection}";
        }

        public static void Remove<T>(this IList<T> list, Type type)
        {
            var instances = list.Where(x => x.GetType() == type).ToList();
            instances.ForEach(obj => list.Remove(obj));
        }

        public static IEnumerable<T> All<T>(this IQueryCollection collection, string key)
        {
            var values = new List<T>();

            if (collection.TryGetValue(key, out var results))
            {
                foreach (var s in results)
                {
                    try
                    {
                        var result = (T) Convert.ChangeType(s, typeof(T));
                        values.Add(result);
                    }
                    catch (Exception)
                    {
                        // conversion failed
                        // skip value
                    }
                }
            }

            // return an array with at least one
            return values;
        }

        public static T Get<T>(
            this IQueryCollection collection,
            string key,
            T @default = default,
            ParameterPick option = ParameterPick.First)
        {
            var values = All<T>(collection, key);
            var value = @default;

            if (values.Any())
            {
                value = option switch
                {
                    ParameterPick.First => values.FirstOrDefault(),
                    ParameterPick.Last => values.LastOrDefault(),
                    _ => value
                };
            }

            return value ?? @default;
        }

        public enum ParameterPick
        {
            First,
            Last
        }
    }
}