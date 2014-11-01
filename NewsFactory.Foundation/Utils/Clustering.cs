using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFactory.Foundation.Utils
{
    public static class Clustering
    {
        #region Methods

        public static List<double[]> SelectSeedClusters(int clustersCount, List<double[]> items)
        {
            return SelectRandomSeeds(clustersCount, items).Select(i =>
            {
                var t = new double[items[i].Length];
                Array.Copy(items[i], t, items[i].Length);
                return t;
            }).ToList();
        }

        public static List<double[]> Cluster(int clustersCount, List<double[]> items)
        {
            var clusters = SelectSeedClusters(clustersCount, items);
            var assignments = new int[items.Count];
            var iterations = 0;
            while (iterations < 10)
            {
                var reassignments = 0;
                // Eval cluster assignments
                for (var i = 0; i < items.Count; i++)
                {
                    var newAssignment = ArgMin(clusters.Select(c => GetDistance(c, items[i])));
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
                    AddVectors(clusters[assignments[i]], items[i]);
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
            return clusters;
        }

        public static int[] Cluster(List<double[]> clusters, List<double[]> items)
        {
            var assignments = new int[items.Count];
            var reassignments = 0;

            // Eval cluster assignments
            for (var i = 0; i < items.Count; i++)
            {
                var newAssignment = ArgMin(clusters.Select(c => GetDistance(c, items[i])));
                if (assignments[i] != newAssignment)
                {
                    assignments[i] = newAssignment;
                    reassignments++;
                }
            }

            foreach (var cluster in clusters)
            {
                for (int i = 0; i < cluster.Length; i++)
                {
                    cluster[i] = 0;
                }
            }
            for (int i = 0; i < items.Count; i++)
            {
                AddVectors(clusters[assignments[i]], items[i]);
            }
            for (int i = 0; i < clusters.Count; i++)
            {
                var ac = (double)assignments.Count(t => t == i);
                for (int j = 0; j < clusters[i].Length; j++)
                {
                    clusters[i][j] /= ac;
                }
            }
            return assignments;
        }

        static double GetDistance(double[] v1, double[] v2)
        {
            var sum = 0.0;
            for (int i = 0; i < v1.Length; i++)
            {
                sum += (v1[i] - v2[i]) * (v1[i] - v2[i]);
            }
            return Math.Sqrt(sum);
        }

        static void AddVectors(double[] v1, double[] v2)
        {
            for (int i = 0; i < v1.Length; i++)
            {
                v1[i] += v2[i];
            }
        }

        static IEnumerable<int> SelectRandomSeeds(int clustersCount, List<double[]> points)
        {
            var rnd = new Random();
            var seeds = new HashSet<int>();
            while (seeds.Count < clustersCount)
            {
                var i = rnd.Next(points.Count);
                if (!seeds.Contains(i))
                    seeds.Add(i);
            }
            return seeds;
        }

        static int ArgMin<T>(IEnumerable<T> list) where T : IComparable<T>
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

        #endregion Methods
    }
}
