using Microsoft.VisualStudio.Text.Editor;
using System.Windows;

public partial class SimpleButtonMargin : IWpfTextViewMargin
{
    private string name;
    private SimpleButton button;

    public SimpleButtonMargin(string name, SimpleButton button)
    {
        this.name = name;
        this.button = button;
    }

    public FrameworkElement VisualElement => button;

    public double MarginSize => VisualElement.ActualHeight;

    public bool Enabled => true;


    public ITextViewMargin GetTextViewMargin(string marginName) => marginName == this.name ? this : null;

    public void Dispose()
    {
    }
}

