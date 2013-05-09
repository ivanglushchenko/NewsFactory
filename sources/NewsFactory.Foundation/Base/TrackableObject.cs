using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFactory.Foundation.Base
{
    public partial class TrackableObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string pn)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(pn));
        }

        #endregion INotifyPropertyChanged Implementation
    }
}
