using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WFsoft.ExHttpClient
{
    public static class ExWebClient
    {
        /// <summary>
        /// 下载网址包括本地文件到指定路径,主要用于单个本地文件显示拷贝进度,进度委托(最大值,进度值,进度百分比)
        /// </summary>
        /// <param name="address">请求地址</param>
        /// <param name="saveFileName">保存文件全路径</param>
        /// <param name="progressValueAction">进度委托,最大值100</param>
        /// <returns></returns>
        public static Task ExDownFileAsync(this WebClient webClient, string address, string saveFileName, Action<string,string,int> progressAction)
        {
            void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) => progressAction?.Invoke(ExCommon.HumanReadableFilesize(e.TotalBytesToReceive), ExCommon.HumanReadableFilesize(e.BytesReceived), e.ProgressPercentage);
            webClient.DownloadProgressChanged += DownloadProgressChanged;
            return webClient.DownloadFileTaskAsync(address, saveFileName).ContinueWith(x=> {
                webClient.DownloadProgressChanged -= DownloadProgressChanged;
                x.GetAwaiter().GetResult();
            });
        }
    }
}
