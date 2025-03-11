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

        public static void Remove<T>(this IList<T> list, Type type)
        {
            var instances = list.Where(x => x.GetType() == type).ToList();
            instances.ForEach(obj => list.Remove(obj));
        }
    }
}
