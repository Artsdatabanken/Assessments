using System.Web;
using Microsoft.AspNetCore.Http.Extensions;

namespace Assessments.Web.Infrastructure;

public static class QueryStringExtensions
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

    public static string RemoveQueryStringValue(this IQueryCollection query, string key, string value)
    {
        var items = query.SelectMany(x => x.Value, (y, z) => new KeyValuePair<string, string>(y.Key, z)).ToList();

        items.RemoveAll(x => x.Key == key && x.Value == value);

        var queryBuilder = new QueryBuilder(items);

        return queryBuilder.ToQueryString().Value;
    }

    public static string RemoveQueryStringKeys(this Uri uri, string[] keys)
    {
        var queryString = HttpUtility.ParseQueryString(uri.Query);

        var dictionary = queryString.AllKeys.ToDictionary(x => x, name => queryString[name]);

        foreach (var x in dictionary.Where(y => string.IsNullOrEmpty(y.Value) || keys.Contains(y.Key)))
            queryString.Remove(x.Key);

        var uriBuilder = new UriBuilder(uri) { Query = queryString.ToString() ?? string.Empty };

        return uriBuilder.ToString();
    }
}