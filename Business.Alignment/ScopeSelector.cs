using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;

namespace FileUtil
{
    public class ScopeSelector
    {
        public string ScopeSelectorRegex { get; set; }
        public int? Start { get; set; }
        public int? End { get; set; }

        public IEnumerable<ITextSnapshotLine> GetLinesToAlign(Document view)
        {
            int start = Start ?? view.StartSelectionLineNumber;
            int end = End ?? view.EndSelectionLineNumber;

            if (start == end) return new List<ITextSnapshotLine>();

            //if (start == end)
            //{
            //    var blanks = start.DownTo(0).Where(x => IsLineBlank(view, x));
            //    start = blanks.Any() ? blanks.First() + 1 : 0;

            //    blanks = end.UpTo(view.LineCount - 1).Where(x => IsLineBlank(view, x));
            //    end = blanks.Any() ? blanks.First() - 1 : view.LineCount - 1;
            //}

            return start.UpTo(end).Select(x => view.GetLineFromLineNumber(x));
        }

        bool IsLineBlank(Document view, int lineNo)
        {
            var text = view.GetLineFromLineNumber(lineNo).GetText();
            return text.Length == 0;
            //return text.Length == 0 || Regex.IsMatch(text, ScopeSelectorRegex);
        }
    }
}

