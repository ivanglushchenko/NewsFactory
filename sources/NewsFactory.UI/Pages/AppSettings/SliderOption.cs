using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFactory.UI.Pages.AppSettings
{
    public class SliderOption
    {
        #region .ctors

        static SliderOption()
        {
            Days = new SliderOption[]
            {
                new SliderOption() { Caption = "1 day", Interval = 1, Value = 0 },
                new SliderOption() { Caption = "2 days", Interval = 2, Value = 2 },
                new SliderOption() { Caption = "3 days", Interval = 3, Value = 4 },
                new SliderOption() { Caption = "4 days", Interval = 4, Value = 6 },
                new SliderOption() { Caption = "1 week", Interval = 7, Value = 8 },
                new SliderOption() { Caption = "1 month", Interval = 30, Value = 10 },
            };
            Intervals = new SliderOption[]
            {
                new SliderOption() { Caption = "every 30 minutes", Interval = 30, Value = 0 },
                new SliderOption() { Caption = "every 1 hour", Interval = 60, Value = 2 },
                new SliderOption() { Caption = "every 3 hours", Interval = 180, Value = 4 },
                new SliderOption() { Caption = "every 6 hours", Interval = 360, Value = 6 },
                new SliderOption() { Caption = "every day", Interval = 1440, Value = 8 },
                new SliderOption() { Caption = "never", Interval = 0, Value = 10 }
            };
        }

        #endregion .ctors

        #region Properties

        public static SliderOption[] Days { get; private set; }
        public static SliderOption[] Intervals { get; private set; }

        public string Caption { get; set; }
        public int Interval { get; set; }
        public int Value { get; set; }

        #endregion Properties
    }
}
