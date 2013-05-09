using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFactory.Foundation.Model
{
    [Flags]
    public enum FeedStatus
    {
        None = 0,
        New = 1,
        Active = 2,
        HasDefaultFavIcon = 4
    }
}
