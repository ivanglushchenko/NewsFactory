using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFactory.Foundation.Model
{
    [Flags]
    public enum ItemStatus
    {
        None = 0,
        IsNew = 1,
        Like = 2,
        Dislike = 4,
        ReadLater = 8,
        Favorite = 16,
        ClassifiedAsLike = 32,
        ClassifiedAsDislike = 64
    }
}
