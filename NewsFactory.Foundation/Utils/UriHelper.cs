using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFactory.Foundation.Utils
{
    public static class UriHelper
    {
        #region Methods

        public static Uri ToUri(this string url)
        {
            if (url == null) return null;

            Uri source;
            if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out source))
            {
                return null;
            }
            return source;
        }

        #endregion Methods
    }
}
