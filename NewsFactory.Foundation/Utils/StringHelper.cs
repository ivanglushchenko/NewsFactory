using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFactory.Foundation.Utils
{
    public static class StringHelper
    {
        #region Methods

        public static string Beautify(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;

            StringBuilder sbc = new StringBuilder(s);
            sbc.Replace("&#150;", "-");
            sbc.Replace("&#151;", "—");
            sbc.Replace("&#160;", " ");
            sbc.Replace("&gt;", ">");
            sbc.Replace("&lt;", "<");
            sbc.Replace("&quot;", "\"");
            sbc.Replace("&nbsp;", " ");
            sbc.Replace("&amp;", "&");
            sbc.Replace("&#39;", "'");
            sbc.Replace("&#039;", "'");
            sbc.Replace("&#0039;", "'");
            sbc.Replace("&hellip;", "...");
            sbc.Replace("&mdash;", "-");
            sbc.Replace("&#8211;", "-");
            sbc.Replace("&#8212;", "—");
            sbc.Replace("&#8216;", "'");
            sbc.Replace("&#8217;", "'");
            sbc.Replace("&#8220;", "\"");
            sbc.Replace("&#8221;", "\"");
            sbc.Replace("&#8230;", "...");
            sbc.Replace("&rsquo;", "'");
            return sbc.ToString();
        }

        public static string Encode(this string s)
        {
            return new string(s.Replace('.', '_').Replace("://", "_").Replace('/', '_').ToCharArray().Where(c => c == '_' || char.IsLetterOrDigit(c)).ToArray());
        }

        #endregion Methods
    }
}