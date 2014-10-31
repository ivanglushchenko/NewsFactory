using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFactory.Foundation.Utils
{
    public static class Tokenizer
    {
        #region Fields

        static readonly char[] _sentenceTerminators = new char[] { '.', ',', ';', '!', '?', ':', '(', ')', '[', ']', '{', '}', '"', '“', '”', '‘', '…', '\n', '\r', '\t', '/', '«', '»', '_', '—', '<', '>' };

        #endregion Fields

        #region Methods

        public static IEnumerable<string> ExtractText(string text)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.OptionFixNestedTags = true;
                doc.OptionAutoCloseOnEnd = true;
                doc.LoadHtml(text);
                return GetTextEntries(doc.DocumentNode.ChildNodes);
            }
            catch
            {
            }
            return new string[0];
        }

        public static IEnumerable<string> TokenizeHtml(string text)
        {
            return ExtractText(text).SelectMany(SplitTerms);
        }

        public static bool IsGoodTerm(string term)
        {
            if (term.Length < 1)
                return false;

            if (term.Length == 1)
                return char.IsLetterOrDigit(term[0]);

            if (term.Length > 25)
                return false;

            if (term.StartsWith("&") || term.EndsWith("&"))
                return false;

            if (term.Contains("\\") || term.Contains("="))
                return false;

            if (term.StartsWith("?"))
                return false;

            return true;
        }

        static IEnumerable<string> GetTextEntries(IEnumerable<HtmlNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.Name.ToLower() == "#text")
                    yield return node.InnerText.Beautify();
                else
                {
                    foreach (var item in GetTextEntries(node.ChildNodes))
                    {
                        yield return item;
                    }
                }
            }
        }

        static IEnumerable<string> SplitTerms(string body)
        {
            foreach (var item in SplitSentences(body))
            {
                foreach (var term in item.Split(' ', (char)160))
                {
                    var v = term.Trim(' ', '’', '\'');
                    if (v.EndsWith("'s") || v.EndsWith("’s") || v.EndsWith("'S") || v.EndsWith("’S"))
                        v = v.Substring(0, v.Length - 2);
                    if (IsGoodTerm(v))
                        yield return v.ToLowerInvariant();
                }
            }
        }

        static IEnumerable<string> SplitSentences(string body)
        {
            var iStart = 0;
            while (iStart < body.Length)
            {
                var iEnd = body.IndexOfAny(_sentenceTerminators, iStart);
                if (iEnd < 0)
                {
                    // Return the rest of the string
                    iEnd = body.Length;
                }
                else if (iEnd == iStart)
                {
                    // Skip the dot
                    iStart = iEnd + 1;
                    continue;
                }
                else
                {
                    var addOne = false;
                    // handle abbreviations like U.K.
                    while (iEnd > iStart &&
                           iEnd < (body.Length - 2) &&
                           char.IsUpper(body[iEnd - 1]) &&
                           body[iEnd] == '.' &&
                           char.IsUpper(body[iEnd + 1]) &&
                           body[iEnd + 2] == '.')
                    {
                        iEnd += 2;
                        addOne = true;
                    }
                    if (addOne)
                        iEnd++;
                }

                yield return body.Substring(iStart, iEnd - iStart);

                iStart = iEnd + 1;
            }
        }

        #endregion Methods
    }
}
