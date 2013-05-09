using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NewsFactory.Foundation.Services
{
    public class ClassifierService
    {
        #region Methods

        public async Task MakeTestCall()
        {
            var httpClient = new HttpClient();
            var content = await httpClient.GetStringAsync("http://localhost:8080/");
        }

        public async Task SendTrainingSet(string sessionID)
        {
            var httpClient = new HttpClient();
            var httpContent = new StringContent("hello world!");
            var response = await httpClient.PostAsync(string.Format("http://localhost:8080/ts/{0}", sessionID), httpContent);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
            }
        }

        #endregion Methods
    }
}