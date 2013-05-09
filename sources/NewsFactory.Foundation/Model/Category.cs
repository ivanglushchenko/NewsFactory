using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFactory.Foundation.Model
{
    [Flags]
    public enum Category : long
    {
        None =                0x0,
        Animals =             0x1,
        ArtAndPhotography =   0x2,
        BestOfYouTube =       0x4,
        Books =               0x8,
        Business =            0x10,
        Cars =                0x20,
        CelebrityGossip =     0x40,
        College =             0x80,
        ComicBooks =          0x100,
        Dating =              0x200,
        Deals =               0x400,
        Design =              0x800,
        Fashion =             0x1000,
        FoodAndWine =         0x2000,
        Gaming =              0x4000,
        Gardening =           0x8000,
        Health =              0x10000,
        Humor =               0x20000,
        Movies =              0x40000,
        Music =               0x80000,
        Politics =            0x100000,
        Programming =         0x200000,
        Science =             0x400000,
        Sports =              0x800000,
        Technology =          0x1000000,
        Travel =              0x2000000,
        TV =                  0x4000000,
        USNews =              0x8000000,
        WorldNews =           0x10000000,
        Yoga =                0x20000000,

        Custom1 =             0x1000000000,
        Custom2 =             0x2000000000,
        Custom3 =             0x3000000000,

        All = int.MaxValue
    }
}
