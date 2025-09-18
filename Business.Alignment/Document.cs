using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace FileUtil
{
    public class Document
    {
        IWpfTextView doc;
        ITextSnapshot snapshot;

        public Document(IWpfTextView doc)
        {
            this.doc = doc;
            snapshot = doc.TextSnapshot;
        }

        public int LineCount
        {
            get { return snapshot.LineCount; }
        }

        public int StartSelectionLineNumber
        {
            get { return snapshot.GetLineNumberFromPosition(doc.Selection.Start.Position); }
        }

        public int EndSelectionLineNumber
        {
            get { return snapshot.GetLineNumberFromPosition(doc.Selection.End.Position); }
        }

        public int CaretColumn
        {
            get
            {
                var caret = doc.Caret.Position.BufferPosition;
                var index = doc.GetTextViewLineContainingBufferPosition(caret).Start.Difference(caret);
                var line = snapshot.GetLineFromPosition(caret).GetText().Substring(0, index);
                return line.ReplaceTabs(TabSize).Length + doc.Caret.Position.VirtualSpaces;
            }
        }

        public bool ConvertTabsToSpaces
        {
            get { return doc.Options.GetOptionValue(DefaultOptions.ConvertTabsToSpacesOptionId); }
        }

        public int TabSize
        {
            get { return doc.Options.GetOptionValue(DefaultOptions.TabSizeOptionId); }
        }

        public ITextSnapshotLine GetLineFromLineNumber(int lineNo)
        {
            return snapshot.GetLineFromLineNumber(lineNo);
        }

        public ITextEdit StartEdit()
        {
            return snapshot.TextBuffer.CreateEdit();
        }

        public string FileType
        {
            get { return "." + doc.TextBuffer.ContentType.TypeName.ToLower(); }
        }

        public void Refresh()
        {
            snapshot = doc.TextSnapshot;
        }
    }
}