using NewsFactory.Foundation.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public void AddRange(IList<NewsItem> items)
        {
            var sw = new Stopwatch();
            sw.Start();
            foreach (var item in items)
            {
                item.SimiliarItems = null;
                item.IsHeadNewsItem = true;
                item.IsChildNewsItem = false;
            }
            foreach (var item in items)
            {
                Add(item);
            }
            foreach (var item in items.Where(t => t.SimiliarItems != null && t.IsHeadNewsItem).ToList())
            {
                var i = items.IndexOf(item);
                foreach (var child in item.SimiliarItems)
                {
                    items.Remove(child);
                    items.Insert(++i, child);
                }
            }
            sw.Stop();
            System.Diagnostics.Debug.WriteLine("Elapsed: {0}", sw.Elapsed);
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
                    {
                        existingItem.Key.SimiliarItems = new ObservableCollection<NewsItem>();
                    }

                    existingItem.Key.SimiliarItems.Add(item);
                    item.SimiliarItems = existingItem.Key.SimiliarItems;
                    item.IsHeadNewsItem = false;
                    item.IsChildNewsItem = true;
                    break;
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
