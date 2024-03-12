using System.ComponentModel;
using Nuke.Common.Tooling;

[TypeConverter(typeof(TypeConverter<BuildConfiguration>))]
public class BuildConfiguration : Enumeration
{
    public static BuildConfiguration Debug = new() { Value = nameof(Debug) };
    public static BuildConfiguration Release = new() { Value = nameof(Release) };

    public static implicit operator string(BuildConfiguration buildConfiguration)
    {
        return buildConfiguration.Value;
    }

    public override string ToString()
    {
        return this;
    }
}
