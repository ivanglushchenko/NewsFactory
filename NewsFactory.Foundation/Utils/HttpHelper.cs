﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace NewsFactory.Foundation.Utils
{
    public sealed class HttpHelper
    {
        public IAsyncOperation<IDictionary<string, string>> PostToUriAsync(string targetUri, string content)
        {
            return GeneralHelper.Run<IDictionary<string, string>>(async () =>
            {
                var headers = await SendPostToUriAsync(targetUri, content);
                return headers;

            }).AsAsyncOperation();
        }

        private async Task<IDictionary<string, string>> SendPostToUriAsync(string targetUri, string content)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;
            handler.AllowAutoRedirect = false;

            HttpClient client = new HttpClient(handler);

            HttpContent httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            HttpResponseMessage response = await client.PostAsync(targetUri, httpContent);

            Dictionary<string, string> headers = new Dictionary<string, string>();

            foreach (var header in response.Headers)
            {
                string headerValue = string.Empty;

                if (header.Value.Count() == 1)
                {
                    headerValue = header.Value.First();
                }
                else
                {
                    StringBuilder stringValue = new StringBuilder();

                    foreach (var value in header.Value)
                    {
                        stringValue.Append(value);
                        stringValue.Append("\n");
                    }

                    headerValue = stringValue.ToString();
                }

                headers.Add(header.Key, headerValue);
            }

            return headers;
        }
    }
}
