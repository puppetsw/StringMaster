using StringMaster.UI.Strings;

namespace StringMaster.UI.Helpers;

public static class StringHelpers
{
    public static string GetLocalizedString(string name)
    {
        var resourceMgr = ResourceStrings.ResourceManager;
        var str = resourceMgr.GetString(name);
        return str;
    }
}
