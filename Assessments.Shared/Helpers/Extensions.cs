using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Assessments.Shared.Helpers;

public static class Extensions
{
    public static string DisplayName(this MemberInfo property)
    {
        var attribute = property.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Single();

        return attribute.DisplayName;
    }

    private static T GetAttribute<T>(this Enum value) where T : Attribute
    {
        var type = value.GetType();
        var memberInfo = type.GetMember(value.ToString());
        var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);

        if (attributes.Length > 0)
            return (T) attributes[0];

        return null;
    }

    public static string Description(this Enum value)
    {
        var attribute = value.GetAttribute<DisplayAttribute>();

        if (attribute != null)
        {
            return attribute switch
            {
                { ResourceType: not null, Description: not null } => ResourceManagerHelper.GetManager(attribute.ResourceType).GetString(attribute.Description),
                _ => attribute.Description ?? value.ToString()
            };
        }

        var descriptionAttribute = value.GetAttribute<DescriptionAttribute>();
            
        return descriptionAttribute == null ? value.ToString() : descriptionAttribute.Description;
    }

    public static string DisplayName(this Enum value)
    {
        var attribute = value.GetAttribute<DisplayAttribute>();

        return attribute switch
        {
            { ResourceType: not null, Name: not null } => ResourceManagerHelper.GetManager(attribute.ResourceType).GetString(attribute.Name),
            _ => attribute?.Name ?? value.ToString()
        };
    }

    public static TEnum ConvertTo<TEnum>(this Enum source)
    {
        try
        {
            return (TEnum)Enum.Parse(typeof(TEnum), source.ToString(), ignoreCase: true);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidOperationException
            (
                $"Could not convert {source.GetType()} [{source}] to {typeof(TEnum)}", ex
            );
        }
    }

    public static IEnumerable<T> ToEnumerable<T>(this IEnumerable<string> array)
    {
        return array.Where(c => Enum.IsDefined(typeof(T), c)).Select(a => (T) Enum.Parse(typeof(T), a));
    }
}