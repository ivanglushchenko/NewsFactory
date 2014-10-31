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
            _newsItemsVectors.Add(item, normalizedVector);
        }

        public void PrepareWordIndex(int wordCountThreshold, int documentCountThreshold)
        {
            _activeWords =
                _words
                .Where(t => t.Value.WordCount >= wordCountThreshold && t.Value.DocumentCount >= documentCountThreshold)
                .Select(t => t.Value.Index)
                .ToArray();
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

        public void Cluster(int clustersCount)
        {
            var items = _newsItemsUniformVectors.Keys.ToList();
            var clusters = SelectRandomSeeds(clustersCount).Select(i => _newsItemsUniformVectors[items[i]]).ToArray();
            var assignments = new int[items.Count];
            var iterations = 0;
            while (iterations < 10)
            {
                var reassignments = 0;
                // Eval cluster assignments
                for (var i = 0; i < items.Count; i++)
                {
                    var newAssignment = ArgMin(clusters.Select(c => GetDistance(c, _newsItemsUniformVectors[items[i]])));
                    if (assignments[i] != newAssignment)
                    {
                        assignments[i] = newAssignment;
                        reassignments++;
                    }
                }
                if (reassignments == 0)
                    break;
                // Recalc clusters
                foreach (var cluster in clusters)
                {
                    for (int i = 0; i < cluster.Length; i++)
                    {
                        cluster[i] = 0;
                    }
                }
                for (int i = 0; i < items.Count; i++)
                {
                    AddVectors(clusters[assignments[i]], _newsItemsUniformVectors[items[i]]);
                }
                for (int i = 0; i < clustersCount; i++)
                {
                    var ac = (double)assignments.Count(t => t == i);
                    for (int j = 0; j < clusters[i].Length; j++)
                    {
                        clusters[i][j] /= ac;
                    }
                }

                iterations++;
            }
        }

        double GetDistance(double[] v1, double[] v2)
        {
            var sum = 0.0;
            for (int i = 0; i < v1.Length; i++)
            {
                sum += (v1[i] - v2[i]) * (v1[i] - v2[i]);
            }
            return Math.Sqrt(sum);
        }

        void AddVectors(double[] v1, double[] v2)
        {
            for (int i = 0; i < v1.Length; i++)
            {
                v1[i] += v2[i];
            }
        }

        IEnumerable<int> SelectRandomSeeds(int clustersCount)
        {
            var rnd = new Random();
            var seeds = new HashSet<int>();
            while (seeds.Count < clustersCount)
            {
                var i = rnd.Next(_newsItemsUniformVectors.Count);
                if (!seeds.Contains(i))
                    seeds.Add(i);
            }
            return seeds;
        }

        int ArgMin<T>(IEnumerable<T> list) where T : IComparable<T>
        {
            var minValue = default(T);
            var minIndex = -1;
            var currentIndex = 0;

            foreach (var item in list)
            {
                if (currentIndex == 0)
                {
                    minValue = item;
                    minIndex = 0;
                }
                else
                {
                    if (minValue.CompareTo(item) > 0)
                    {
                        minValue = item;
                        minIndex = currentIndex;
                    }
                }
                currentIndex++;
            }

            if (minIndex < 0)
                throw new ArgumentException("list");

            return minIndex;
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
