using HtmlAgilityPack;
using NewsFactory.Foundation.Components;
using NewsFactory.Foundation.Controls;
using NewsFactory.Foundation.Model;
using NewsFactory.Foundation.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace NewsFactory.Foundation.Utils
{
    public class XamlConverter
    {
        #region .ctors

        public XamlConverter()
        {
        }

        public XamlConverter(Dictionary<string, Tuple<double, double>> imageSizes)
        {
            _imageSizes = imageSizes;
        }

        #endregion .ctors

        #region Events

        public static event Action<NewsItem, List<Paragraph>> XamlUpdated;

        #endregion Events

        #region Fields

        private static object _syncObject = new object();
        private List<Paragraph> _paragraphs;
        private List<Inline> _rootLevelInlines;
        private NewsItem _newsItem;
        private List<Image> _imagesToLoad = new List<Image>();
        private Dictionary<string, Tuple<double, double>> _imageSizes = new Dictionary<string, Tuple<double, double>>();

        #endregion Fields

        #region Methods

        public List<Paragraph> BuildXaml(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return new List<Paragraph>();

            //var names = typeof(DataService).GetTypeInfo().Assembly.GetManifestResourceNames();
            //var name = names.First(t => t.EndsWith("TextFile1.txt"));
            //var stream = typeof(DataService).GetTypeInfo().Assembly.GetManifestResourceStream(name);
            //content = new StreamReader(stream).ReadToEnd();

            var doc = new HtmlDocument();
            doc.OptionFixNestedTags = true;
            doc.OptionAutoCloseOnEnd = true;
            doc.LoadHtml(content);

            _paragraphs = new List<Paragraph>() { new Paragraph() };
            _rootLevelInlines = null;

            foreach (var item in Convert(doc.DocumentNode.ChildNodes))
            {
                _paragraphs.Last().Inlines.Add(item);
            }

            return _paragraphs;
        }

        public void AssignXamlToItem(NewsItem newsItem)
        {
            if (newsItem == null) return;
            if (newsItem.DescriptionXaml != null) return;

            lock (_syncObject)
            {
                _newsItem = newsItem;
                _newsItem.DescriptionXaml = BuildXaml(_newsItem.Description);

                foreach (var item in _imagesToLoad)
                {
                    item.ImageOpened += OnProcessImage;
                    item.ImageFailed += OnProcessImage;
                }
            }
        }

        void OnProcessImage(object sender, RoutedEventArgs e)
        {
            var img = (Image)sender;
            img.ImageOpened -= OnProcessImage;
            img.ImageFailed -= OnProcessImage;

            var s = img.Source as BitmapImage;
            if (s != null)
            {
                img.Width = s.PixelWidth;
                img.Height = s.PixelHeight;
                _imageSizes[(string)img.Tag] = new Tuple<double, double>(s.PixelWidth, s.PixelHeight);
            }

            lock (_syncObject)
            {
                if (_imagesToLoad.Contains(img))
                    _imagesToLoad.Remove(img);

                if (_imagesToLoad.Count == 0)
                {
                    var newXaml = new XamlConverter(_imageSizes).BuildXaml(_newsItem.Description);

                    _newsItem.DescriptionXamlUpdated = newXaml;
                     if (XamlUpdated != null)
                     {
                         XamlUpdated(_newsItem, newXaml);
                     }
                }
            }
        }

        /// <summary>
        /// Gets the tag attribute value for specific other attribute name and value.
        /// </summary>
        /// <param name="htmlFragment">The HTML fragment.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="testAttributeName">Name of the test attribute.</param>
        /// <param name="testAttributeValue">The test attribute value.</param>
        /// <param name="attributeToGet">The attribute to get.</param>
        /// <returns></returns>
        public static List<string> GetTagAttributeBySpecificAttribute(
            string htmlFragment,
            string tagName,
            string testAttributeName,
            string testAttributeValue,
            string attributeToGet)
        {
            var regex = new Regex(
                string.Format(
                    "\\<{0}[^\\>]+{1}=\\\"{2}\\\"[^\\>]+{3}=\\\"(?<retGroup>[^\\>]+?)\\\"",
                    tagName,
                    testAttributeName,
                    testAttributeValue,
                    attributeToGet),
                    RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var matches = regex.Matches(htmlFragment).OfType<Match>().ToList();
            if (matches.Count > 0)
                return matches.Select(m => m.Groups["retGroup"].Value).ToList();

            regex = new Regex(
                string.Format(
                    "\\<{0}[^\\>]+{3}=\\\"(?<retGroup>[^\\>]+?)\\\"[^\\>]+{1}=\\\"{2}\\\"",
                    tagName,
                    testAttributeName,
                    testAttributeValue,
                    attributeToGet),
                    RegexOptions.Multiline | RegexOptions.IgnoreCase);
            matches = regex.Matches(htmlFragment).OfType<Match>().ToList();
            if (matches.Count > 0)
                return matches.Select(m => m.Groups["retGroup"].Value).ToList();

            return new List<string>();
        }

        private List<Inline> Convert(HtmlNodeCollection nodes)
        {
            var elements = new List<Inline>();

            if (_rootLevelInlines == null) _rootLevelInlines = elements;

            foreach (var node in nodes)
            {
                foreach (var item in Convert(elements, node))
                {
                    elements.Add(item);
                }
            }

            return elements;
        }

        private List<Inline> Convert(List<Inline> list, HtmlNode node)
        {
            switch (node.Name.ToLower())
            {
                case "br":
                    return new List<Inline>() { new LineBreak() };

                case "blockquote":
                    return AddAsBlock(node, p => p.Margin = new Thickness(25, 0, 0, 0));

                case "img":
                    return new List<Inline>() { new InlineUIContainer() { Child = GetImage(node) }, new LineBreak() };

                case "a":
                    {
                        var url = node.Attributes["href"] != null ? node.Attributes["href"].Value : null;
                        var url2 = url != null ? new Uri(url, UriKind.RelativeOrAbsolute) : null;
                        var link = new Hyperlink()
                        {
                            Content = Clean(node.InnerText),
                            NavigationUrl = url2 != null && url2.IsAbsoluteUri ? url2 : null
                        };
                        if (node.ChildNodes.Count == 1 && node.ChildNodes.First().Name == "img")
                        {
                            var image = GetImage(node.ChildNodes.First());
                            link.Content = image;
                            link.Width = image.Width;
                            link.Height = image.Height;
                            link.UpdateSize = false;
                            return new List<Inline>() { new InlineUIContainer() { Child = link }, new LineBreak() };
                        }

                        return new List<Inline>() { new InlineUIContainer() { Child = link } };
                    }

                case "p":
                    {
                        var s = new Span();
                        s.Inlines.Add(new LineBreak());
                        foreach (var item in Convert(node.ChildNodes))
                        {
                            s.Inlines.Add(item);
                        }
                        s.Inlines.Add(new LineBreak());
                        return new List<Inline>() { s };
                    }

                case "em":
                case "i":
                    {
                        var i = new Italic();
                        foreach (var item in Convert(node.ChildNodes))
                        {
                            i.Inlines.Add(item);
                        }
                        return new List<Inline>() { i };
                    }

                case "#text":
                    {
                        if (node.InnerText != "\r\n")
                            return new List<Inline>() { new Run() { Text = Clean(node.InnerText) } };
                        return new List<Inline>();
                    }

                case "dl":
                    return ConvertToList(node.ChildNodes, false);

                case "ol":
                    return ConvertToList(node.ChildNodes, true);

                case "ul":
                    return ConvertToList(node.ChildNodes, false);

                case "b":
                case "strong":
                    {
                        var b = new Bold();
                        foreach (var item in Convert(node.ChildNodes))
                        {
                            b.Inlines.Add(item);
                        }
                        return new List<Inline>() { b };
                    }
                case "h1":
                    return ConvertHeader(list, Convert(node.ChildNodes), 24);

                case "h2":
                    return ConvertHeader(list, Convert(node.ChildNodes), 22);

                case "h3":
                    return ConvertHeader(list, Convert(node.ChildNodes), 20);

                case "h4":
                    return ConvertHeader(list, Convert(node.ChildNodes), 18);

                case "h5":
                    return ConvertHeader(list, Convert(node.ChildNodes), 16);

                case "h6":
                    return ConvertHeader(list, Convert(node.ChildNodes), 14);

                case "h7":
                    return ConvertHeader(list, Convert(node.ChildNodes), 12);

                case "hr":
                    return new List<Inline>() { new InlineUIContainer() { Child = new Border() { Background = new SolidColorBrush(Colors.Black), Height = 1, Width = 3000 } } };

                case "table":
                    return new List<Inline>();

                default:
                    return WrapWithSpan(Convert(node.ChildNodes));
            }
        }

        private Image GetImage(HtmlNode node)
        {
            var src = node.Attributes["src"] != null ? node.Attributes["src"].Value : null;
            var width = node.Attributes["width"] != null ? node.Attributes["width"].Value : (node.Attributes["data-src-width"] != null ? node.Attributes["data-src-width"].Value : null);
            var height = node.Attributes["height"] != null ? node.Attributes["height"].Value : (node.Attributes["data-src-height"] != null ? node.Attributes["data-src-height"].Value : null);
            var bi = new BitmapImage();
            bi.UriSource = ToUrl(src);
            var img = new Image() { Source = bi, Stretch = Stretch.None, Tag = src };
            if (ToDouble(width) != null)
                img.Width = ToDouble(width).Value;
            if (ToDouble(height) != null)
                img.Height = ToDouble(height).Value;
            if (_imageSizes.ContainsKey(src ?? string.Empty))
            {
                var s = _imageSizes[src ?? string.Empty];
                img.Width = s.Item1;
                img.Height = s.Item2;
            }
            else if (double.IsNaN(img.Width) || double.IsNaN(img.Height))
            {
                _imagesToLoad.Add(img);
                img.Width = 0;
                img.Height = 0;
            }
            return img;
        }

        private double? ToDouble(string s)
        {
            double d;
            if (double.TryParse(s, out d))
                return d;
            return null;
        }

        private Uri ToUrl(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            var uri = new Uri(s, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri) return uri;

            try
            {
                s = s.StartsWith("//") ? "http:" + s : (s.StartsWith("/") ? "http:/" + s : "http://" + s);
                uri = new Uri(s, UriKind.RelativeOrAbsolute);
                if (uri.IsAbsoluteUri) return uri;
            }
            catch (Exception)
            {
            }

            return null;
        }

        private List<Inline> ConvertHeader(List<Inline> containerList, List<Inline> items, int fontSize)
        {
            var s = new Span() { FontSize = fontSize };
            if (containerList.Count > 0)
            {
                s.Inlines.Add(new LineBreak());
                s.Inlines.Add(new LineBreak());
            }
            foreach (var item in items)
            {
                s.Inlines.Add(item);
            }
            return new List<Inline>() { s };
        }

        private List<Inline> WrapWithSpan(List<Inline> items)
        {
            var s = new Span();
            foreach (var item in items)
            {
                s.Inlines.Add(item);
            }
            return new List<Inline>() { s };
        }

        private Paragraph PushBlock()
        {
            if (_rootLevelInlines.Count > 0)
            {
                foreach (var item in _rootLevelInlines)
                {
                    _paragraphs.Last().Inlines.Add(item);
                }
                _rootLevelInlines.Clear();
            }
            if (_paragraphs.Last().Inlines.Count == 0)
                _paragraphs.Remove(_paragraphs.Last());
            _paragraphs.Add(new Paragraph());
            return _paragraphs.Last();
        }

        private List<Inline> AddAsBlock(HtmlNode node, Action<Paragraph> init = null)
        {
            var p = PushBlock();
            if (init != null)
                init(p);
            foreach (var item in Convert(node.ChildNodes))
                p.Inlines.Add(item);
            _paragraphs.Add(new Paragraph());
            return new List<Inline>();
        }

        private List<Inline> ConvertToList(HtmlNodeCollection nodes, bool showIndices)
        {
            if (nodes.Any(n => n.Name.ToLower() == "ol" || n.Name.ToLower() == "ul" || n.Name.ToLower() == "dl"))
                return ConvertToList(nodes.First(n => n.Name.ToLower() == "ol" || n.Name.ToLower() == "ul" || n.Name.ToLower() == "dl").ChildNodes, showIndices);

            foreach (var node in nodes.Where(t => t.Name.ToLower() == "li" || t.Name.ToLower() == "dt" || t.Name.ToLower() == "dd"))
            {
                AddAsBlock(node, p => p.Margin = new Thickness(node.Name.ToLower().Equals("dt") ? 15 : 25, 0, 0, 0));
            }
            return new List<Inline>();
        }

        private string Clean(string content)
        {
            return StringHelper.Beautify(content);
        }

        #endregion Methods
    }
}
