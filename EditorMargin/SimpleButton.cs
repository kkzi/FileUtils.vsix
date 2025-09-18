using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

public partial class SimpleButton : Label
{
    public SimpleButton()
    {
        FontSize = 11;
        ClipToBounds = true;
        Padding = new Thickness(5, 0, 5, 0);
        Foreground = getThemeColor(EnvironmentColors.ToolWindowTextColorKey);
        BorderThickness = new Thickness(1);
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;

        resetColors();
        MouseLeave += (sender, args) => resetColors();

        MouseEnter += (sender, args) =>
        {
            Background = getThemeColor(EnvironmentColors.CommandBarMouseOverBackgroundBeginColorKey);
            BorderBrush = getThemeColor(EnvironmentColors.CommandBarSelectedBorderColorKey);

            ThemeResourceKey[] res = {
            };
            foreach (var it in res)
            {
                var color = getThemeColor(it);
                var str = color.ToString();
                if (str.EndsWith("E8E8EC"))
                {
                    str = "";
                }
            }
        };
    }

    private void resetColors()
    {
        BorderBrush = getThemeColor(EnvironmentColors.ScrollBarBackgroundColorKey);
        Background = getThemeColor(EnvironmentColors.ScrollBarBackgroundColorKey);
    }


    private Brush getThemeColor(ThemeResourceKey key)
    {
        var c1 = VSColorTheme.GetThemedColor(key);
        var c2 = Color.FromArgb(c1.A, c1.R, c1.G, c1.B);
        return new SolidColorBrush(c2);
    }
}

