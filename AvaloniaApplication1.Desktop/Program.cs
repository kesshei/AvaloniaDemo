using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Fonts;

namespace AvaloniaApplication1.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static int Main(string[] args)
    {
        var builder = BuildAvaloniaApp();
        if (args.Contains("--drm"))
        {
            SilenceConsole();

            // If Card0, Card1 and Card2 all don't work. You can also try:                 
            // return builder.StartLinuxFbDev(args);
            // return builder.StartLinuxDrm(args, "/dev/dri/card1");
            return builder.StartLinuxDrm(args, "/dev/dri/card1", 1D);
        }

        return builder.StartWithClassicDesktopLifetime(args);
    }


    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseFontAlibaba()
            .WithInterFont()
            .LogToTrace();


    private static void SilenceConsole()
    {
        new Thread(() =>
            {
                Console.CursorVisible = false;
                while (true)
                    Console.ReadKey(true);
            })
        { IsBackground = true }.Start();
    }
}
public static class AvaloniaAppBuilderExtensions
{
    public static AppBuilder UseFontAlibaba([DisallowNull] this AppBuilder builder, Action<FontSettings>? configDelegate = default)
    {
        var setting = new FontSettings();
        configDelegate?.Invoke(setting);

        return builder.With(new FontManagerOptions
        {
            DefaultFamilyName = setting.DefaultFontFamily,
            FontFallbacks = new[]
            {
                new FontFallback
                {
                    FontFamily = new FontFamily(setting.DefaultFontFamily)
                }
            }
        }).ConfigureFonts(manager => manager.AddFontCollection(new EmbeddedFontCollection(setting.Key, setting.Source)));
    }
    public class FontSettings
    {
        public string DefaultFontFamily = "fonts:AvaloniaApplication1FontFamilies#Alibaba PuHuiTi";
        public Uri Key { get; set; } = new Uri("fonts:AvaloniaApplication1FontFamilies", UriKind.Absolute);
        public Uri Source { get; set; } = new Uri("avares://AvaloniaApplication1/Assets/Fonts", UriKind.Absolute);
    }
}
