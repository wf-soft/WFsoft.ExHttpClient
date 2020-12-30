# WFsoft.ExHttpClient

> 首先声明全局变量，第二个参数为超时时间,单位/秒，不设置则为默认值
* HttpClient旨在一次声明全局调用，如果需要改变超时时间，请创建不同的全局变量
```
public static WFsoft.ExHttpClient.ExHttpClient httpClient15 = new WFsoft.ExHttpClient.ExHttpClient(new(), 15);
public static WFsoft.ExHttpClient.ExHttpClient httpClient300 = new WFsoft.ExHttpClient.ExHttpClient(new(), 300);
```

> 以Post方式的FormData发起请求或上传文件
```
httpClient15.ExPostFormDataAsync("http://localhost:8080/aip/upload", new Dictionary<string, string>() {
                {"Key1", "Value1" },
                {"Key2", "Value2" }
            });
```

```
httpClient15.ExPostFormDataAsync("http://localhost:8080/aip/upload", new Dictionary<string, string>() {
                {"Key1", "Value1" },
                {"Key2", "Value2" }
            }, new Dictionary<string, string>(){
                {"File1", "C:\\1.JPG" },
                {"File2", "C:\\2.JPG" }
            });
```

```
httpClient15.ExPostFormDataAsync("http://localhost:8080/aip/upload", new Dictionary<string, string>() {
                {"Key1", "Value1" },
                {"Key2", "Value2" }
            }, new Dictionary<string, string>(){
                {"File1", "C:\\1.JPG" },
                {"File2", "C:\\2.JPG" }
            },CancellationToken.None);
```

```
httpClient15.ExPostFormDataAsync("http://localhost:8080/aip/upload", new Dictionary<string, string>() {
                {"Key1", "Value1" },
                {"Key2", "Value2" }
            }, new Dictionary<string, string>(){
                {"File1", "C:\\1.JPG" },
                {"File2", "C:\\2.JPG" }
            },(maxSize, uploadSize, progress)=> {
                Debug.WriteLine(maxSize);
                Debug.WriteLine(uploadSize);
                Debug.WriteLine(progress);
            },CancellationToken.None);
```

* 当然，你也可以使用普通HttpClient, ExHttpClient继承自HttpClient
```
httpClient15.PostAsJsonAsync<T>(usr);
```

> 下载文件
```
httpClient300.ExDownloadToFileAsync(url, savePath, CancellationToken.None);
```
```
httpClient300.ExDownloadToFileAsync(url, savePath, (maxSize, uploadSize, progress) => {
                Debug.WriteLine(maxSize);
                Debug.WriteLine(uploadSize);
                Debug.WriteLine(progress);
            }, CancellationToken.None);
```
```
httpClient300.ExDownloadBytesAsync(url, (maxSize, uploadSize, progress) => {
                Debug.WriteLine(maxSize);
                Debug.WriteLine(uploadSize);
                Debug.WriteLine(progress);
            }, CancellationToken.None);
```

> 构造函数也可以进行一些需要的设置
```
var progressMessageHandler = new ProgressMessageHandler(httpMessageHandler)
public static WFsoft.ExHttpClient.ExHttpClient httpClient15 = new WFsoft.ExHttpClient.ExHttpClient(progressMessageHandler);

```
