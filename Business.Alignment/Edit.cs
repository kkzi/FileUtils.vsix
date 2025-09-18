using Microsoft.VisualStudio.Text;

namespace FileUtil
{
    public class Edit
    {
        ITextEdit edit;

        public Edit(ITextEdit edit)
        {
            this.edit = edit;
        }

        public bool Replace(int start, int len, string text)
        {
            return edit.Replace(new Span(start, len), text);
        }

        public void Commit()
        {
            edit.Apply();
        }

        public void Dispose()
        {
            edit.Dispose();
        }
    }
}