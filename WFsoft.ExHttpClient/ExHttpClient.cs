using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WFsoft.ExHttpClient
{
    /// <summary>
    /// <para>WFsoft类库</para>
    /// <para>依赖 Microsoft.AspNet.WebApi.Client 包</para>
    /// <para>示例：ExHttpClient = new(new ProgressMessageHandler(new HttpClientHandler()))</para>
    /// <para>HttpClient旨在单实例全局复用,如果需要不同的HttpClientHandler或超时时间,请创建另外的实例</para>
    /// </summary>
    public class ExHttpClient :HttpClient
    {
        private readonly ProgressMessageHandler processMessageHander;

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="_defaultTimeOut">默认超时秒数</param>
        /// <param name="_progressMessageHandler">进度消息处理程序</param>
        public ExHttpClient(ProgressMessageHandler _progressMessageHandler, int defaultTimeOut = 15) : base(_progressMessageHandler)
        {
            Timeout = TimeSpan.FromSeconds(defaultTimeOut);
            processMessageHander = _progressMessageHandler;
        }

        /// <summary>
        /// 下载网址到byte[],带进度,进度委托(最大值,进度值,进度百分比)
        /// </summary>
        /// <param name="address">请求地址</param>
        /// <param name="progressAction">进度委托</param>
        /// <param name="token">取消Token</param>
        /// <returns></returns>
        public Task<byte[]> ExDownloadBytesAsync(string address, Action<string,string,int> progressAction, CancellationToken token = default)
        {
            void HttpReceiveProgress(object sender, HttpProgressEventArgs e) => progressAction?.Invoke(ExCommon.HumanReadableFilesize(e.TotalBytes), ExCommon.HumanReadableFilesize(e.BytesTransferred), e.ProgressPercentage);
            processMessageHander.HttpReceiveProgress += HttpReceiveProgress;
            return GetByteArrayAsync(address,token).ContinueWith(x=> {
                processMessageHander.HttpReceiveProgress -= HttpReceiveProgress;
                return x.GetAwaiter().GetResult();
            });
        }

        /// <summary>
        /// 下载网址到文件,带进度,只能下载http和https开头地址,进度委托(最大值,进度值,进度百分比)
        /// </summary>
        /// <param name="address">请求地址</param>
        /// <param name="saveFileName">保存文件全路径</param>
        /// <param name="progressAction">进度委托</param>
        /// <param name="token">取消Token</param>
        /// <returns></returns>
        public async Task ExDownloadToFileAsync(string address, string saveFileName, Action<string,string,int> progressAction,  CancellationToken token = default)
        {
            await File.WriteAllBytesAsync(saveFileName, await ExDownloadBytesAsync(address, progressAction, token), token);
        }

        /// <summary>
        /// 下载网址到文件,只能下载http和https开头地址
        /// </summary>
        /// <param name="address">请求地址</param>
        /// <param name="saveFileName">保存文件全路径</param>
        /// <param name="token">取消Token</param>
        /// <returns></returns>
        public async Task ExDownloadToFileAsync(string address, string saveFileName, CancellationToken token = default) {
            await File.WriteAllBytesAsync(saveFileName, await GetByteArrayAsync(address, token), token);
        }

        /// <summary>
        /// 上传文件不带进度
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="Fields">字段集合,不可null</param>
        /// <param name="Files">文件集合,可null</param>
        /// <param name="token">取消Token</param>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExPostFormDataAsync(string url, Dictionary<string, string> Fields, Dictionary<string, string> Files = null, CancellationToken token = default)
        {
            MultipartFormDataContent formdata = new MultipartFormDataContent();
            foreach (var field in Fields)
                formdata.Add(new StringContent(field.Value), field.Key);
            var files = Files?.Select(x => new { Key = x.Key, FileName = x.Value, StreamVal = File.OpenRead(x.Value) }).ToList();
            if (files is not null) files.ForEach(x => formdata.Add(new StreamContent(x.StreamVal), x.Key, Path.GetFileName(x.FileName)));
            return PostAsync(url, formdata, token).ContinueWith(x=> {
                if (files is not null) files.ForEach(x => x.StreamVal.Close());
                return x.GetAwaiter().GetResult();
            });
        }

        /// <summary>
        /// 上传文件带进度,进度委托(最大值,进度值,进度百分比)
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="Fields">字段集合,不可null</param>
        /// <param name="Files">文件集合,不可null</param>
        /// <param name="progressAction">进度委托,不可null</param>
        /// <param name="token">取消Token</param>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExPostFormDataAsync(string url, Dictionary<string, string> Fields, Dictionary<string, string> Files, Action<string,string,int> progressAction, CancellationToken token = default)
        {
            void HttpSendProgress(object sender, HttpProgressEventArgs e) => progressAction?.Invoke(ExCommon.HumanReadableFilesize(e.TotalBytes), ExCommon.HumanReadableFilesize(e.BytesTransferred), e.ProgressPercentage);
            processMessageHander.HttpSendProgress += HttpSendProgress;
            MultipartFormDataContent formdata = new MultipartFormDataContent();
            foreach (var field in Fields) formdata.Add(new StringContent(field.Value), field.Key);
            var files = Files?.Select(x => new { Key = x.Key, FileName = x.Value, StreamVal = File.OpenRead(x.Value) }).ToList();
            files.ForEach(x => formdata.Add(new StreamContent(x.StreamVal), x.Key, Path.GetFileName(x.FileName)));
            return PostAsync(url, formdata, token).ContinueWith(x=> {
                processMessageHander.HttpSendProgress -= HttpSendProgress;
                files.ForEach(x => x.StreamVal.Close());
                return x.GetAwaiter().GetResult();
            });
        }
    }
}
