using NewsFactory.Foundation.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;

namespace NewsFactory.Foundation.Model
{
    [Bindable]
    public partial class NewsItem : TrackableObject, IComparable<NewsItem>
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
        /// Gets/sets FeedUrl.
        /// </summary>
        public Uri FeedUrl
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_FeedUrl; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_FeedUrl != value)
                {
                    p_FeedUrl = value;
                    OnPropertyChanged("FeedUrl");
                    OnFeedUrlChanged();
                }
            }
        }
        private Uri p_FeedUrl;
        partial void OnFeedUrlChanged();

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
        /// Gets/sets ReceivedAt.
        /// </summary>
        public DateTime ReceivedAt
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_ReceivedAt; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_ReceivedAt != value)
                {
                    p_ReceivedAt = value;
                    OnPropertyChanged("ReceivedAt");
                    OnReceivedAtChanged();
                }
            }
        }
        private DateTime p_ReceivedAt;
        partial void OnReceivedAtChanged();

        /// <summary>
        /// Gets/sets Published.
        /// </summary>
        public DateTime Published
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Published; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Published != value)
                {
                    p_Published = value;
                    OnPropertyChanged("Published");
                    OnPublishedChanged();
                }
            }
        }
        private DateTime p_Published;
        partial void OnPublishedChanged();

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
        /// Gets/sets Status.
        /// </summary>
        public ItemStatus Status
        {
            get { return p_Status; }
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
        private ItemStatus p_Status;
        partial void OnStatusChanged();

        [IgnoreDataMember]
        public NewsFeed Feed { get; set; }

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

                    Correlate(value, ItemStatus.IsNew);
                }
            }
        }
        [IgnoreDataMember]
        private bool p_IsNew;
        partial void OnIsNewChanged();

        /// <summary>
        /// Gets/sets IsLike.
        /// </summary>
        [IgnoreDataMember]
        public bool IsLike
        {
            get { return p_IsLike; }
            set
            {
                if (p_IsLike != value)
                {
                    p_IsLike = value;
                    OnPropertyChanged("IsLike");
                    OnIsLikeChanged();

                    Correlate(value, ItemStatus.Like);
                }
            }
        }
        [IgnoreDataMember]
        private bool p_IsLike;
        partial void OnIsLikeChanged();

        /// <summary>
        /// Gets/sets IsDislike.
        /// </summary>
        [IgnoreDataMember]
        public bool IsDislike
        {
            get { return p_IsDislike; }
            set
            {
                if (p_IsDislike != value)
                {
                    p_IsDislike = value;
                    OnPropertyChanged("IsDislike");
                    OnIsDislikeChanged();

                    Correlate(value, ItemStatus.Dislike);
                }
            }
        }
        [IgnoreDataMember]
        private bool p_IsDislike;
        partial void OnIsDislikeChanged();

        /// <summary>
        /// Gets/sets IsReadLater.
        /// </summary>
        [IgnoreDataMember]
        public bool IsReadLater
        {
            get { return p_IsReadLater; }
            set
            {
                if (p_IsReadLater != value)
                {
                    p_IsReadLater = value;
                    OnPropertyChanged("IsReadLater");
                    OnIsReadLaterChanged();

                    Correlate(value, ItemStatus.ReadLater);
                }
            }
        }
        [IgnoreDataMember]
        private bool p_IsReadLater;
        partial void OnIsReadLaterChanged();

        /// <summary>
        /// Gets/sets IsFavorite.
        /// </summary>
        [IgnoreDataMember]
        public bool IsFavorite
        {
            get { return p_IsFavorite; }
            set
            {
                if (p_IsFavorite != value)
                {
                    p_IsFavorite = value;
                    OnPropertyChanged("IsFavorite");
                    OnIsFavoriteChanged();

                    Correlate(value, ItemStatus.Favorite);
                }
            }
        }
        [IgnoreDataMember]
        private bool p_IsFavorite;
        partial void OnIsFavoriteChanged();

        /// <summary>
        /// Gets/sets IsClassifiedAsLike.
        /// </summary>
        [IgnoreDataMember]
        public bool IsClassifiedAsLike
        {
            get { return p_IsClassifiedAsLike; }
            set
            {
                if (p_IsClassifiedAsLike != value)
                {
                    p_IsClassifiedAsLike = value;
                    OnPropertyChanged("IsClassifiedAsLike");
                    OnIsClassifiedAsLikeChanged();

                    Correlate(value, ItemStatus.ClassifiedAsLike);
                }
            }
        }
        [IgnoreDataMember]
        private bool p_IsClassifiedAsLike;
        partial void OnIsClassifiedAsLikeChanged();

        /// <summary>
        /// Gets/sets IsClassifiedAsDislike.
        /// </summary>
        [IgnoreDataMember]
        public bool IsClassifiedAsDislike
        {
            get { return p_IsClassifiedAsDislike; }
            set
            {
                if (p_IsClassifiedAsDislike != value)
                {
                    p_IsClassifiedAsDislike = value;
                    OnPropertyChanged("IsClassifiedAsDislike");
                    OnIsClassifiedAsDislikeChanged();

                    Correlate(value, ItemStatus.ClassifiedAsDislike);
                }
            }
        }
        [IgnoreDataMember]
        private bool p_IsClassifiedAsDislike;
        partial void OnIsClassifiedAsDislikeChanged();

        /// <summary>
        /// Gets/sets DescriptionXaml.
        /// </summary>
        [IgnoreDataMember]
        public List<Paragraph> DescriptionXaml
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_DescriptionXaml; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_DescriptionXaml != value)
                {
                    p_DescriptionXaml = value;
                    OnPropertyChanged("DescriptionXaml");
                    OnDescriptionXamlChanged();
                }
            }
        }
        [IgnoreDataMember]
        private List<Paragraph> p_DescriptionXaml;
        partial void OnDescriptionXamlChanged();

        /// <summary>
        /// Gets/sets DescriptionXamlUpdated.
        /// </summary>
        [IgnoreDataMember]
        public List<Paragraph> DescriptionXamlUpdated
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_DescriptionXamlUpdated; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_DescriptionXamlUpdated != value)
                {
                    p_DescriptionXamlUpdated = value;
                    OnPropertyChanged("DescriptionXamlUpdated");
                    OnDescriptionXamlUpdatedChanged();
                }
            }
        }
        [IgnoreDataMember]
        private List<Paragraph> p_DescriptionXamlUpdated;
        partial void OnDescriptionXamlUpdatedChanged();

        /// <summary>
        /// Gets/sets SimiliarItems.
        /// </summary>
        [IgnoreDataMember]
        public ObservableCollection<NewsItem> SimiliarItems
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_SimiliarItems; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_SimiliarItems != value)
                {
                    p_SimiliarItems = value;
                    OnPropertyChanged("SimiliarItems");
                    OnSimiliarItemsChanged();
                }
            }
        }
        [IgnoreDataMember]
        private ObservableCollection<NewsItem> p_SimiliarItems;
        partial void OnSimiliarItemsChanged();

        /// <summary>
        /// Gets/sets IsHeadNewsItem.
        /// </summary>
        public bool IsHeadNewsItem
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_IsHeadNewsItem; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_IsHeadNewsItem != value)
                {
                    p_IsHeadNewsItem = value;
                    OnPropertyChanged("IsHeadNewsItem");
                    OnIsHeadNewsItemChanged();
                }
            }
        }
        private bool p_IsHeadNewsItem = true;
        partial void OnIsHeadNewsItemChanged();

        /// <summary>
        /// Gets/sets IsChildNewsItem.
        /// </summary>
        public bool IsChildNewsItem
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_IsChildNewsItem; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_IsChildNewsItem != value)
                {
                    p_IsChildNewsItem = value;
                    OnPropertyChanged("IsChildNewsItem");
                    OnIsChildNewsItemChanged();
                }
            }
        }
        private bool p_IsChildNewsItem;
        partial void OnIsChildNewsItemChanged();

        /// <summary>
        /// Gets/sets RenderingMode.
        /// </summary>
        [IgnoreDataMember]
        public ItemRenderingMode RenderingMode
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _RenderingMode; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_RenderingMode != value)
                {
                    _RenderingMode = value;
                    OnPropertyChanged("RenderingMode");
                    OnRenderingModeChanged();
                }
            }
        }
        [IgnoreDataMember]
        ItemRenderingMode _RenderingMode;
        partial void OnRenderingModeChanged();

        #endregion Properties

        #region Methods

        public int CompareTo(NewsItem other)
        {
            if (other == null) return 1;

            if (ReceivedAt > other.ReceivedAt) return 1;
            if (ReceivedAt == other.ReceivedAt)
            {
                var r = -string.Compare(FeedUrl.ToString(), other.FeedUrl.ToString());
                if (r > 0) return 1;
                if (r == 0)
                {
                    if (Published > other.Published) return 1;
                    if (Published == other.Published)
                    {
                        return -string.Compare(Title, other.Title);
                    }
                    else
                        return -1;
                }
                else
                    return -1;
            }
            else
                return -1;
        }

        public override string ToString()
        {
            return string.Format("Item: {0}", Title);
        }

        public bool Is(ItemStatus status)
        {
            return (Status & status) == status;
        }

        private void Correlate(bool value, ItemStatus itemStatus)
        {
            if (value)
                Status |= itemStatus;
            else
                Status &= ~itemStatus;
        }

        partial void OnStatusChanged()
        {
            IsNew = Is(ItemStatus.IsNew);
            IsLike = Is(ItemStatus.Like);
            IsDislike = Is(ItemStatus.Dislike);
            IsReadLater = Is(ItemStatus.ReadLater);
            IsFavorite = Is(ItemStatus.Favorite);
            IsClassifiedAsLike = Is(ItemStatus.ClassifiedAsLike);
            IsClassifiedAsDislike = Is(ItemStatus.ClassifiedAsDislike);

            RenderingMode = IsNew ? ItemRenderingMode.NotSelectedNew : ItemRenderingMode.NotSelectedOld;
        }

        public bool MarkAsRead()
        {
            if (IsNew)
            {
                IsNew = false;
                Feed.NewItemsCount--;
                Feed.Store.All.NewItemsCount--;
                Feed.Store.Unread.NewItemsCount--;
                return true;
            }
            return false;
        }

        public bool MarkAsUnread()
        {
            if (!IsNew)
            {
                IsNew = true;
                Feed.NewItemsCount++;
                Feed.Store.All.NewItemsCount++;
                Feed.Store.Unread.NewItemsCount++;
                return true;
            }
            return false;
        }

        #endregion Methods
    }

    public enum ItemRenderingMode
    {
        Selected,
        NotSelectedNew,
        NotSelectedOld
    }
}
