using System.Windows.Controls;
using System.Windows.Input;

namespace GraphicsSandbox.App.UI;
/// <summary>
/// Interaction logic for MainView.xaml
/// </summary>
public partial class EditorView : UserControl
{
    public EditorView()
    {
        InitializeComponent();
    }

    private void TextBox_ReplaceTabWithSpaces(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Tab)
        {
            e.Handled = true;
            var textBox = (TextBox)sender;
            int caretIndex = textBox.CaretIndex;
            textBox.Text = textBox.Text.Insert(caretIndex, "    ");
            textBox.CaretIndex = caretIndex + 4;
        }
    }
}
