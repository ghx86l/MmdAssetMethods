using System.Text;
using System.Runtime.CompilerServices;

namespace MmdAssetMethods;

/// <summary>
/// <para>MMD関係で使用するエンコード方式</para>
/// <para>Shift JIS はそのままだと使えないので</para>
/// </summary>
public static class Encoding
{
    private static bool ProviderIsRegistered { get; set; }

    [ModuleInitializer]
    public static void Initialize()
    {
        System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        ProviderIsRegistered = true;
    }

    public static System.Text.Encoding CP932 => Get(932);

    public static System.Text.Encoding ShiftJIS => CP932;

    public static System.Text.Encoding UTF8 => CP65001;

    public static System.Text.Encoding CP936 => Get(936);

    public static System.Text.Encoding CP949 => Get(949);

    public static System.Text.Encoding CP950 => Get(950);

    public static System.Text.Encoding CP1252 => Get(1252);

    public static System.Text.Encoding CP65001 => Get(65001);

    public static System.Text.Encoding Get(int codePage)
    {
        if (!ProviderIsRegistered)
        {
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ProviderIsRegistered = true;
        }

        return System.Text.Encoding.GetEncoding(codePage);
    }

    public static System.Text.Encoding Get(string name)
    {
        if (!ProviderIsRegistered)
        {
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ProviderIsRegistered = true;
        }
        return System.Text.Encoding.GetEncoding(name);
    }
}
