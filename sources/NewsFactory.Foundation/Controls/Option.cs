using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFactory.Foundation.Controls
{
    public class Option
    {
        #region Properties

        public string Caption { get; set; }
        public int Value { get; set; }
        internal int SliderValue { get; set; }

        #endregion Properties
    }
}
