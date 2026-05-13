namespace OwlPost.Core.Utilities;

public static class ApplicationSetting
{
    public static bool IsDebugMode
    {
        get
        {
#if DEBUG
            return true;
#endif

            return false;
        }
    }

    public static bool IsLogEnabled => false;   

}