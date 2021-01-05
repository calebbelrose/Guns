using System;
using System.Reflection;
public class StringValueAttribute : Attribute
{

    #region Properties

    public string StringValue { get; protected set; }

    #endregion

    #region Constructor
    public StringValueAttribute(string value)
    {
        this.StringValue = value;
    }

    #endregion

    public static string GetStringValue(Enum value)
    {
        // Get the type
        Type type = value.GetType();

        // Get fieldinfo for this type
        FieldInfo fieldInfo = type.GetField(value.ToString());

        // Get the stringvalue attributes
        StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
            typeof(StringValueAttribute), false) as StringValueAttribute[];

        // Return the first if there was a match.
        return attribs.Length > 0 ? attribs[0].StringValue : null;
    }
}
