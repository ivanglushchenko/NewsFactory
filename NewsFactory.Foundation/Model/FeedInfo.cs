using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace NewsFactory.Foundation.Model
{
    [Bindable]
    public partial class FeedInfo : TrackableObject
    {
        #region Properties

        /// <summary>
        /// Gets/sets Url.
        /// </summary>
        public Uri Url
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Url; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Url != value)
                {
                    p_Url = value;
                    OnPropertyChanged("Url");
                    OnUrlChanged();
                }
            }
        }
        private Uri p_Url;
        partial void OnUrlChanged();

        /// <summary>
        /// Gets/sets Title.
        /// </summary>
        public string Title
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Title; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Title != value)
                {
                    p_Title = value;
                    OnPropertyChanged("Title");
                    OnTitleChanged();
                }
            }
        }
        private string p_Title;
        partial void OnTitleChanged();

        /// <summary>
        /// Gets/sets Description.
        /// </summary>
        public string Description
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Description; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Description != value)
                {
                    p_Description = value;
                    OnPropertyChanged("Description");
                    OnDescriptionChanged();
                }
            }
        }
        private string p_Description;
        partial void OnDescriptionChanged();

        /// <summary>
        /// Gets/sets ImageUrl.
        /// </summary>
        public Uri ImageUrl
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_ImageUrl; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_ImageUrl != value)
                {
                    p_ImageUrl = value;
                    OnPropertyChanged("ImageUrl");
                    OnImageUrlChanged();
                }
            }
        }
        private Uri p_ImageUrl;
        partial void OnImageUrlChanged();

        /// <summary>
        /// Gets/sets FavIconUrl.
        /// </summary>
        public Uri FavIconUrl
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_FavIconUrl; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_FavIconUrl != value)
                {
                    p_FavIconUrl = value;
                    OnPropertyChanged("FavIconUrl");
                    OnFavIconUrlChanged();
                }
            }
        }
        private Uri p_FavIconUrl;
        partial void OnFavIconUrlChanged();

        /// <summary>
        /// Gets/sets LastPub.
        /// </summary>
        public DateTime LastPub
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_LastPub; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_LastPub != value)
                {
                    p_LastPub = value;
                    OnPropertyChanged("LastPub");
                    OnLastPubChanged();
                }
            }
        }
        private DateTime p_LastPub;
        partial void OnLastPubChanged();

        /// <summary>
        /// Gets/sets Category.
        /// </summary>
        public Category Category
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Category; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Category != value)
                {
                    p_Category = value;
                    OnPropertyChanged("Category");
                    OnCategoryChanged();
                }
            }
        }
        private Category p_Category;
        partial void OnCategoryChanged();

        /// <summary>
        /// Gets/sets Status.
        /// </summary>
        public FeedStatus Status
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Status; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Status != value)
                {
                    p_Status = value;
                    OnPropertyChanged("Status");
                    OnStatusChanged();
                }
            }
        }
        private FeedStatus p_Status;
        partial void OnStatusChanged();

        /// <summary>
        /// Gets/sets IsNew.
        /// </summary>
        [IgnoreDataMember]
        public bool IsNew
        {
            get { return p_IsNew; }
            set
            {
                if (p_IsNew != value)
                {
                    p_IsNew = value;
                    OnPropertyChanged("IsNew");
                    OnIsNewChanged();

                    Correlate(value, FeedStatus.New);
                }
            }
        }
        [IgnoreDataMember]
        private bool p_IsNew;
        partial void OnIsNewChanged();

        /// <summary>
        /// Gets/sets IsActive.
        /// </summary>
        public bool IsActive
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_IsActive; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_IsActive != value)
                {
                    p_IsActive = value;
                    OnPropertyChanged("IsActive");
                    OnIsActiveChanged();

                    Correlate(value, FeedStatus.Active);
                }
            }
        }
        private bool p_IsActive;
        partial void OnIsActiveChanged();

        /// <summary>
        /// Gets/sets HasDefaultFavIcon.
        /// </summary>
        public bool HasDefaultFavIcon
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_HasDefaultFavIcon; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_HasDefaultFavIcon != value)
                {
                    p_HasDefaultFavIcon = value;
                    OnPropertyChanged("HasDefaultFavIcon");
                    OnHasDefaultFavIconChanged();

                    Correlate(value, FeedStatus.HasDefaultFavIcon);
                }
            }
        }
        private bool p_HasDefaultFavIcon;
        partial void OnHasDefaultFavIconChanged();

        #endregion Properties

        #region Methods

        public string GetFileName()
        {
            if (Url == null) return "NULL_URL";
            return StringHelper.Encode(Url.ToString());
        }

        public bool Is(FeedStatus status)
        {
            return (Status & status) == status;
        }

        private void Correlate(bool value, FeedStatus itemStatus)
        {
            if (value)
                Status |= itemStatus;
            else
                Status &= ~itemStatus;
        }

        partial void OnStatusChanged()
        {
            IsNew = Is(FeedStatus.New);
            IsActive = Is(FeedStatus.Active);
            HasDefaultFavIcon = Is(FeedStatus.HasDefaultFavIcon);
        }

        #endregion Methods
    }
}
