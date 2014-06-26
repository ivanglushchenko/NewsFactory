using NewsFactory.Foundation.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFactory.Foundation.Common
{
    public class WordIndex
    {
        #region Fields

        const double SIMILIARITY_THRESHOLD = 0.6;

        readonly object _syncObject = new object();
        static readonly string[] _separators = new string[] { Environment.NewLine, " ", ",", ".", "?" };
        static readonly HashSet<string> _stopwords = new HashSet<string>(new string[] { "the", "a", "an", "to", "of", "in", "and", "on", "for", "with", "at" });
        readonly Dictionary<string, WordInfo> _words = new Dictionary<string, WordInfo>();
        readonly Dictionary<NewsItem, HashSet<int>> _newsItemsVectors = new Dictionary<NewsItem, HashSet<int>>();

        #endregion Fields

        #region Properties
        
        public int WordsCount
        {
            get { return _words.Count; }
        }

        public Dictionary<string, WordInfo> Words
        {
            get { return _words; }
        }

        #endregion Properties

        #region Methods

        public void AddRange(IEnumerable<NewsItem> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void AddRange(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public IEnumerable<string> Add(NewsItem item)
        {
            var words = Add(item.Title);
            var itemVector = WordToVector(words);

            foreach (var existingItem in _newsItemsVectors)
            {
                var sim = GetSimiliarity(itemVector, existingItem.Value);
                if (sim > SIMILIARITY_THRESHOLD)
                {
                    if (existingItem.Key.SimiliarItems == null)
                        existingItem.Key.SimiliarItems = new ObservableCollection<NewsItem>();
                    existingItem.Key.SimiliarItems.Add(item);
                    item.SimiliarItems = existingItem.Key.SimiliarItems;
                }
            }

            _newsItemsVectors.Add(item, WordToVector(words));

            return words;
        }

        public IEnumerable<string> Add(string line)
        {
            var words = ToWords(line).Where(t => !_stopwords.Contains(t)).ToArray();
            foreach (var item in words)
            {
                if (!_words.ContainsKey(item))
                    _words.Add(item, new WordInfo() { Index = _words.Count, Count = 1 });
                else
                    _words[item].Count += 1;
            }
            return words;
        }

        public HashSet<int> WordToVector(string line)
        {
            var words = ToWords(line);
            return WordToVector(words);
        }

        public HashSet<int> WordToVector(IEnumerable<string> words)
        {
            return new HashSet<int>(words.Where(t => _words.ContainsKey(t)).Select(t => _words[t].Index));
        }

        public void DiscoverSimiliarItems(NewsItem newsItem)
        {
            if (newsItem.SimiliarItems != null)
                return;

            //var r = 
            //    _newsItemsVectors
            //    .Where(t => t.Key != newsItem)
            //    .Select(t => new Tuple<NewsItem, double>(t.Key, GetSimiliarity(t.Key, newsItem)))
            //    .Where(t => t.Item2 >= 0.6)
            //    .OrderByDescending(t => t.Item2)
            //    .ToList();
            //newsItem.SimiliarNewsItemsCount = r.Count;
            //newsItem.SimiliarItems = new ObservableCollection<NewsItem>(r.Select(t => t.Item1));

            //foreach (var item in r)
            //{
            //    if (item.Item1.SimiliarItems != null)
            //        throw new NotSupportedException();

            //    item.Item1.SimiliarItems = newsItem.SimiliarItems;
            //    item.Item1.SimiliarNewsItemsCount = r.Count;
            //}
        }

        IEnumerable<string> ToWords(string line)
        {
            return line.ToLower().Split(_separators, StringSplitOptions.RemoveEmptyEntries);
        }

        double GetSimiliarity(HashSet<int> v1, HashSet<int> v2)
        {
            var simCount = v1.Count(t => v2.Contains(t));
            var totCount = v1.Count + v2.Count(t => !v1.Contains(t));
            var sim =  (double)simCount / (double)totCount;
            return sim;
        }

        #endregion Methods

        #region Internal Classes

        public class WordInfo
        {
            public int Index { get; set; }
            public int Count { get; set; }

            public override string ToString()
            {
                return string.Format("Count: {0}", Count);
            }
        }

        #endregion Internal Classes
    }
}
