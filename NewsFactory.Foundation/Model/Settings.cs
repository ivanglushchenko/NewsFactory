﻿using NewsFactory.Foundation.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace NewsFactory.Foundation.Model
{
    [Bindable]
    public partial class Settings : TrackableObject
    {
        #region Properties

        /// <summary>
        /// Gets/sets UniqueID.
        /// </summary>
        public string UniqueID
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_UniqueID; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_UniqueID != value)
                {
                    p_UniqueID = value;
                    OnPropertyChanged("UniqueID");
                    OnUniqueIDChanged();
                }
            }
        }
        private string p_UniqueID;
        partial void OnUniqueIDChanged();

        /// <summary>
        /// Gets/sets SupportsClassification.
        /// </summary>
        public bool SupportsClassification
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_SupportsClassification; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_SupportsClassification != value)
                {
                    p_SupportsClassification = value;
                    OnPropertyChanged("SupportsClassification");
                    OnSupportsClassificationChanged();
                }
            }
        }
        private bool p_SupportsClassification;
        partial void OnSupportsClassificationChanged();

        /// <summary>
        /// Gets/sets ShowReadLaterGroup.
        /// </summary>
        public bool ShowReadLaterGroup
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _ShowReadLaterGroup; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_ShowReadLaterGroup != value)
                {
                    _ShowReadLaterGroup = value;
                    OnPropertyChanged("ShowReadLaterGroup");
                    OnShowReadLaterGroupChanged();
                }
            }
        }
        bool _ShowReadLaterGroup;
        partial void OnShowReadLaterGroupChanged();

        /// <summary>
        /// Gets/sets ShowBookmarksGroup.
        /// </summary>
        public bool ShowBookmarksGroup
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _ShowBookmarksGroup; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_ShowBookmarksGroup != value)
                {
                    _ShowBookmarksGroup = value;
                    OnPropertyChanged("ShowBookmarksGroup");
                    OnShowBookmarksGroupChanged();
                }
            }
        }
        bool _ShowBookmarksGroup = true;
        partial void OnShowBookmarksGroupChanged();

        /// <summary>
        /// Gets/sets ShowAnalyzeGroup.
        /// </summary>
        public bool ShowAnalyzeGroup
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _ShowAnalyzeGroup; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_ShowAnalyzeGroup != value)
                {
                    _ShowAnalyzeGroup = value;
                    OnPropertyChanged("ShowAnalyzeGroup");
                    OnShowAnalyzeGroupChanged();
                }
            }
        }
        bool _ShowAnalyzeGroup = true;
        partial void OnShowAnalyzeGroupChanged();

        /// <summary>
        /// Gets/sets UseWebView.
        /// </summary>
        public bool UseWebView
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_UseWebView; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_UseWebView != value)
                {
                    p_UseWebView = value;
                    OnPropertyChanged("UseWebView");
                    OnUseWebViewChanged();
                }
            }
        }
        private bool p_UseWebView;
        partial void OnUseWebViewChanged();

        /// <summary>
        /// Gets/sets LastUpdated.
        /// </summary>
        public DateTime? LastUpdated
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_LastUpdated; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_LastUpdated != value)
                {
                    p_LastUpdated = value;
                    OnPropertyChanged("LastUpdated");
                    OnLastUpdatedChanged();
                }
            }
        }
        private DateTime? p_LastUpdated;
        partial void OnLastUpdatedChanged();

        /// <summary>
        /// Gets/sets DeleteOldNews.
        /// </summary>
        public bool DeleteOldNews
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_DeleteOldNews; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_DeleteOldNews != value)
                {
                    p_DeleteOldNews = value;
                    OnPropertyChanged("DeleteOldNews");
                    OnDeleteOldNewsChanged();
                }
            }
        }
        private bool p_DeleteOldNews = true;
        partial void OnDeleteOldNewsChanged();

        /// <summary>
        /// Gets/sets OldNewsDayThreshold.
        /// </summary>
        public int OldNewsDayThreshold
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_OldNewsDayThreshold; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_OldNewsDayThreshold != value && value > 0)
                {
                    p_OldNewsDayThreshold = value;
                    OnPropertyChanged("OldNewsDayThreshold");
                    OnOldNewsDayThresholdChanged();
                }
            }
        }
        private int p_OldNewsDayThreshold = 3;
        partial void OnOldNewsDayThresholdChanged();

        /// <summary>
        /// Gets/sets UpdateInterval.
        /// </summary>
        public int UpdateInterval
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_UpdateInterval; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_UpdateInterval != value)
                {
                    p_UpdateInterval = value;
                    OnPropertyChanged("UpdateInterval");
                    OnUpdateIntervalChanged();
                }
            }
        }
        private int p_UpdateInterval = 180;
        partial void OnUpdateIntervalChanged();

        public int MaxNumberOfConcurrentTasks
        {
            get { return _MaxNumberOfConcurrentTasks; }
            set
            {
                if (_MaxNumberOfConcurrentTasks != value)
                {
                    _MaxNumberOfConcurrentTasks = value;
                    OnPropertyChanged("MaxNumberOfConcurrentTasks");
                    OnMaxNumberOfConcurrentTasksChanged();
                }
            }
        }
        private int _MaxNumberOfConcurrentTasks = 50;
        partial void OnMaxNumberOfConcurrentTasksChanged();

        /// <summary>
        /// Gets/sets FeedOrderMode.
        /// </summary>
        public FeedOrderMode FeedOrderMode
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_FeedOrderMode; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_FeedOrderMode != value)
                {
                    p_FeedOrderMode = value;
                    OnPropertyChanged("FeedOrderMode");
                    OnFeedOrderModeChanged();
                }
            }
        }
        private FeedOrderMode p_FeedOrderMode;
        partial void OnFeedOrderModeChanged();

        /// <summary>
        /// Gets/sets SecondaryTileUpdateInterval.
        /// </summary>
        public int SecondaryTileUpdateInterval
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_SecondaryTileUpdateInterval; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_SecondaryTileUpdateInterval != value)
                {
                    p_SecondaryTileUpdateInterval = value;
                    OnPropertyChanged("SecondaryTileUpdateInterval");
                    OnSecondaryTileUpdateIntervalChanged();
                }
            }
        }
        private int p_SecondaryTileUpdateInterval = 60;
        partial void OnSecondaryTileUpdateIntervalChanged();

        public static Settings Instance { get; set; }

        #endregion Properties

        #region Methods

        public Func<NewsItem, bool> GetNewsFilter()
        {
            var now = DateTime.Now;
            return newsItem =>
            {
                if (DeleteOldNews == false) return true;
                if ((now - newsItem.ReceivedAt).TotalDays <= OldNewsDayThreshold)
                    return true;
                else
                    return false;
            };
        }

        #endregion Methods
    }
}
