using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Linq;

namespace FileUtil
{
    public class Alignment
    {
        private Document doc;
        private ScopeSelector selector;

        public Alignment(IWpfTextView view)
        {
            doc = new Document(view);
            selector = new ScopeSelector();
        }

        public void ByString(string token)
        {
            var lines = selector.GetLinesToAlign(doc);
            if (lines.Count() == 0) return;
            var splitLines = lines.Select(line => line.GetText().ReplaceTabs(doc.TabSize).Split(new[] { token }, StringSplitOptions.None));

            int maxColumns = splitLines.Max(parts => parts.Length);
            int[] maxLengths = new int[maxColumns];

            for (int i = 0; i < maxColumns; i++)
            {
                maxLengths[i] = splitLines.Where(parts => parts.Length > i)
                    .Max(parts => i + 1 == parts.Count() ? 0 : parts[i].Length);
            }

            var replaced = string.Join(Environment.NewLine, splitLines.Select(parts =>
            {
                return string.Join(token, parts.Select((part, index) =>
                    part.PadRight(index < maxLengths.Length && index < parts.Length - 1 ? maxLengths[index] : 0)
                    )
                );
            }));

            var first = lines.First().Start;
            var last = lines.Last().End;
            var edit = doc.StartEdit();
            edit.Replace(first, last - first, replaced);
            edit.Apply();
        }
    }
}