using StringMaster.Strings;

namespace StringMaster.Helpers;

public static class StringHelpers
{
    public static string GetLocalizedString(string name)
    {
        var resourceMgr = ResourceStrings.ResourceManager;
        var str = resourceMgr.GetString(name);
        return str;
    }
}
