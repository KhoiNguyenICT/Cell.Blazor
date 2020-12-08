// Decompiled with JetBrains decompiler
// Type: Syncfusion.Blazor.Data.HttpHandler
// Assembly: Syncfusion.Blazor, Version=18.3.0.52, Culture=neutral, PublicKeyToken=null
// MVID: C2354C79-3B80-43BF-8FE6-0DBACD7553E4
// Assembly location: C:\Users\KhoiNguyenICT\source\repos\BlazorApp4\BlazorApp4\bin\Debug\net5.0\Syncfusion.Blazor.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cell.Blazor.Data.Class
{
    public class HttpHandler : HttpHandlerBase
    {
        public HttpHandler(HttpClient _client)
          : base(_client)
        {
        }

        public HttpClient Client { get; set; }

        public override async Task<HttpResponseMessage> SendRequest(
          HttpRequestMessage data)
        {
            HttpHandler httpHandler = this;
            httpHandler.Client = httpHandler.GetClient();
            HttpResponseMessage returnData = new HttpResponseMessage();
            int num;
            try
            {
                returnData = await httpHandler.Client.SendAsync(data);
                if (returnData.IsSuccessStatusCode)
                    return returnData;
                returnData.EnsureSuccessStatusCode();
                return (HttpResponseMessage)null;
            }
            catch (Exception ex)
            {
                num = 1;
            }
            if (num == 1)
            {
                Exception e = ex;
                HttpContent httpContent = returnData == null ? (HttpContent)null : returnData.Content;
                string message;
                if (httpContent == null)
                    message = "";
                else
                    message = await httpContent.ReadAsStringAsync();
                throw new HttpRequestException(e.Message, new Exception(message));
            }
            returnData = (HttpResponseMessage)null;
            HttpResponseMessage httpResponseMessage;
            return httpResponseMessage;
        }

        public HttpRequestMessage PrepareRequest(RequestOptions options)
        {
            if (!options.Url.StartsWith("http"))
                options.Url = DataUtil.GetUrl(options.BaseUrl, options.Url);
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri(options.Url),
                Method = options.RequestMethod
            };
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add((JsonConverter)new StringEnumConverter());
            settings.NullValueHandling = NullValueHandling.Ignore;
            if (httpRequestMessage.Method.Equals(HttpMethod.Patch))
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            string content = options.Data == null ? "" : (options.Data.GetType() == typeof(string) ? (string)options.Data : JsonConvert.SerializeObject(options.Data, Formatting.Indented, settings));
            if (httpRequestMessage.Method != HttpMethod.Get && httpRequestMessage.Method != HttpMethod.Head)
            {
                StringContent stringContent = new StringContent(content, Encoding.UTF8, options.ContentType);
                httpRequestMessage.Content = (HttpContent)stringContent;
            }
            return httpRequestMessage;
        }

        public HttpRequestMessage PrepareBatchRequest(RequestOptions options)
        {
            if (!options.Url.StartsWith("http"))
                options.Url = DataUtil.GetUrl(options.BaseUrl, options.Url);
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri(options.Url),
                Method = options.RequestMethod
            };
            MultipartContent multipartContent1 = new MultipartContent("mixed", options.ContentType);
            CRUDModel<object> batchRecords = options.Data as CRUDModel<object>;
            int num1 = 0;
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add((JsonConverter)new StringEnumConverter());
            settings.NullValueHandling = NullValueHandling.Ignore;
            CRUDModel<object> crudModel1 = batchRecords;
            int? count;
            int num2;
            if (crudModel1 == null)
            {
                num2 = 0;
            }
            else
            {
                count = crudModel1.Added?.Count;
                int num3 = 0;
                num2 = count.GetValueOrDefault() > num3 & count.HasValue ? 1 : 0;
            }
            if (num2 != 0)
            {
                foreach (object obj in batchRecords.Added)
                {
                    MultipartContent multipartContent2 = new MultipartContent("mixed", options.CSet);
                    HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, options.BaseUrl);
                    httpRequest.Content = (HttpContent)new StringContent(JsonConvert.SerializeObject(obj, Formatting.None, settings), Encoding.UTF8, "application/json");
                    httpRequest.Headers.Add("Accept", options.Accept);
                    httpRequest.Headers.Add("Content-Id", num1.ToString());
                    ++num1;
                    HttpMessageContent httpMessageContent = new HttpMessageContent(httpRequest);
                    httpMessageContent.Headers.Remove("Content-Type");
                    httpMessageContent.Headers.Add("Content-Type", "application/http");
                    httpMessageContent.Headers.Add("Content-Transfer-Encoding", "binary");
                    multipartContent2.Add((HttpContent)httpMessageContent);
                    multipartContent1.Add((HttpContent)multipartContent2);
                }
            }
            CRUDModel<object> crudModel2 = batchRecords;
            int num4;
            if (crudModel2 == null)
            {
                num4 = 0;
            }
            else
            {
                count = crudModel2.Changed?.Count;
                int num3 = 0;
                num4 = count.GetValueOrDefault() > num3 & count.HasValue ? 1 : 0;
            }
            int i;
            if (num4 != 0)
            {
                for (i = 0; i < batchRecords.Changed.Count; i++)
                {
                    MultipartContent multipartContent2 = new MultipartContent("mixed", options.CSet);
                    object val = DataUtil.GetVal((IEnumerable)batchRecords.Changed, i, options.keyField);
                    string odataUrlKey = DataUtil.GetODataUrlKey((object)null, options.keyField, val);
                    HttpRequestMessage httpRequest = new HttpRequestMessage(options.UpdateType, options.BaseUrl + odataUrlKey);
                    List<object> objectList = options.Original is IEnumerable original ? original.Cast<object>().ToList<object>().Where<object>((Func<object, bool>)(e => DataUtil.GetVal((IEnumerable)batchRecords.Changed, i, options.keyField)?.ToString() == e.GetType().GetProperty(options.keyField).GetValue(e)?.ToString())).ToList<object>() : (List<object>)null;
                    httpRequest.Content = (HttpContent)new StringContent(JsonConvert.SerializeObject(DataUtil.CompareAndRemove(batchRecords.Changed[i], objectList?[0], options.keyField), Formatting.None, settings), Encoding.UTF8, "application/json");
                    httpRequest.Headers.Add("Accept", options.Accept);
                    httpRequest.Headers.Add("Content-Id", num1.ToString());
                    ++num1;
                    HttpMessageContent httpMessageContent = new HttpMessageContent(httpRequest);
                    httpMessageContent.Headers.Remove("Content-Type");
                    httpMessageContent.Headers.Add("Content-Type", "application/http");
                    httpMessageContent.Headers.Add("Content-Transfer-Encoding", "binary");
                    multipartContent2.Add((HttpContent)httpMessageContent);
                    multipartContent1.Add((HttpContent)multipartContent2);
                }
            }
            CRUDModel<object> crudModel3 = batchRecords;
            int num5;
            if (crudModel3 == null)
            {
                num5 = 0;
            }
            else
            {
                count = crudModel3.Deleted?.Count;
                int num3 = 0;
                num5 = count.GetValueOrDefault() > num3 & count.HasValue ? 1 : 0;
            }
            if (num5 != 0)
            {
                foreach (object rowData in batchRecords.Deleted)
                {
                    MultipartContent multipartContent2 = new MultipartContent("mixed", options.CSet);
                    string odataUrlKey = DataUtil.GetODataUrlKey(rowData, options.keyField);
                    HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Delete, options.BaseUrl + odataUrlKey);
                    httpRequest.Content = (HttpContent)new StringContent(JsonConvert.SerializeObject(rowData, Formatting.None, settings), Encoding.UTF8, "application/json");
                    httpRequest.Headers.Add("Accept", "application/json;odata=light;q=1,application/json;odata=verbose;q=0.5");
                    httpRequest.Headers.Add("Content-Id", num1.ToString());
                    ++num1;
                    HttpMessageContent httpMessageContent = new HttpMessageContent(httpRequest);
                    httpMessageContent.Headers.Remove("Content-Type");
                    httpMessageContent.Headers.Add("Content-Type", "application/http");
                    httpMessageContent.Headers.Add("Content-Transfer-Encoding", "binary");
                    multipartContent2.Add((HttpContent)httpMessageContent);
                    multipartContent1.Add((HttpContent)multipartContent2);
                }
            }
            httpRequestMessage.Content = (HttpContent)multipartContent1;
            return httpRequestMessage;
        }
    }
}