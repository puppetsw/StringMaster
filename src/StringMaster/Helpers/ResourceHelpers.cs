using StringMaster.Strings;

namespace StringMaster.Helpers;

public static class ResourceHelpers
{
    public static string GetLocalizedString(string name)
    {
        var resourceMgr = ResourceStrings.ResourceManager;
        var str = resourceMgr.GetString(name);
        return str;
    }
}