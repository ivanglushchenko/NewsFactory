using NewsFactory.Foundation.Model;
using NewsFactory.Foundation.Utils;
using System;
using System.Collections;
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
        static readonly HashSet<string> _stopwords = new HashSet<string>(new string[] { "the", "a", "an", "to", "of", "in", "is", "it", "and", "on", "for", "with", "at", "this", "that" });
        readonly Dictionary<string, WordInfo> _words = new Dictionary<string, WordInfo>();
        readonly List<NewsItem> _newsItems = new List<NewsItem>();
        readonly Dictionary<NewsItem, Dictionary<int, double>> _newsItemsVectors = new Dictionary<NewsItem, Dictionary<int, double>>();
        readonly Dictionary<NewsItem, double[]> _newsItemsUniformVectors = new Dictionary<NewsItem, double[]>();

        int[] _activeWords;

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

        public Dictionary<NewsItem, double[]> UniformVectors
        {
            get { return _newsItemsUniformVectors; }
        }

        public int WordCountThreshold { get; set; }

        public int DocumentCountThreshold { get; set; }

        #endregion Properties

        #region Methods

        public void AddRange(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void BuildDictionary(NewsItem item)
        {
            var words = 
                Tokenizer.TokenizeHtml(item.Description)
                .Where(t => !_stopwords.Contains(t))
                .GroupBy(t => t)
                .Select(t => new KeyValuePair<string, int>(t.Key, t.Count()))
                .ToArray();
            foreach (var wordGroup in words)
            {
                var wordInfo = default(WordInfo);
                if (!_words.ContainsKey(wordGroup.Key))
                    _words.Add(wordGroup.Key, wordInfo = new WordInfo() { Index = _words.Count });
                else
                    wordInfo = _words[wordGroup.Key];
                wordInfo.DocumentCount++;
                wordInfo.WordCount += wordGroup.Value;
            }
            var vectorLength = Math.Sqrt(words.Sum(t => t.Value * t.Value));
            var normalizedVector =
                words
                .ToDictionary(
                    t => _words[t.Key].Index,
                    t => (double)t.Value / vectorLength);

            _newsItems.Add(item);
            _newsItemsVectors.Add(item, normalizedVector);
        }

        public async Task PrepareWordIndex(int wordCountThreshold, int documentCountThreshold, Func<Task> increaseProgress)
        {
            if (_activeWords == null || wordCountThreshold != WordCountThreshold || documentCountThreshold != DocumentCountThreshold)
            {
                WordCountThreshold = wordCountThreshold;
                DocumentCountThreshold = documentCountThreshold;
                _activeWords =
                    _words
                    .Where(t => t.Value.WordCount >= wordCountThreshold && t.Value.DocumentCount >= documentCountThreshold)
                    .Select(t => t.Value.Index)
                    .ToArray();

                foreach (var item in _newsItems)
                {
                    AssignUniformVector(item);
                    await increaseProgress();
                }
            }
        }

        public void AssignUniformVector(NewsItem item)
        {
            var vector = _newsItemsVectors[item];
            var uniformVector = new double[_activeWords.Length];
            for (int i = 0; i < _activeWords.Length; i++)
            {
                uniformVector[i] = vector.ContainsKey(_activeWords[i]) ? vector[_activeWords[i]] : 0.0;
            }
            _newsItemsUniformVectors.Add(item, uniformVector);
        }

        public IEnumerable<string> Add(NewsItem item)
        {
            var words = Add(item.Description);
            var itemVector = WordToVector(words);

            //_newsItemsVectors.Add(item, WordToVector(words));

            return words;
        }

        public IEnumerable<string> Add(string line)
        {
            var words = Tokenizer.TokenizeHtml(line).Where(t => !_stopwords.Contains(t)).ToArray();
            foreach (var item in words)
            {
                if (!_words.ContainsKey(item))
                    _words.Add(item, new WordInfo() { Index = _words.Count, WordCount = 1 });
                else
                    _words[item].WordCount += 1;
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
            public int WordCount { get; set; }
            public int DocumentCount { get; set; }

            public override string ToString()
            {
                return string.Format("#Word: {0}, #Doc: {1}", WordCount, DocumentCount);
            }
        }

        #endregion Internal Classes
    }
}
