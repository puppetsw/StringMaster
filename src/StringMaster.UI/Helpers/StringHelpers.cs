using StringMaster.UI.Strings;

namespace StringMaster.UI.Helpers;

public static class StringHelpers
{
	public const string ALL_POINTS = "_All Points";
	public const string NEW_DESCKEY = "New DescKey";
	public const string COPY_OF_ = "Copy of ";

	public static string GetLocalizedString(string name)
	{
		var resourceMgr = ResourceStrings.ResourceManager;
		var str = resourceMgr.GetString(name);
		return str;
	}
}
