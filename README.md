# WebApiEasyInvoker
Very easy to use httpclient for webapi. call remote methods like local methods. support async methods too.

# How to use
1. Install the `WebApiEasyInvoker` nuget package
2. Define a interface which inherit `IWebApiInvoker<T>`
3. Define interface method
4. Chose and use `HttpFullUrlAttribute`,`HttpHostAttribute`,`HttpUrlAttribute` ... to decorate interface or method
5. Use `WebApiExecutionGenerator<T>.Create()` to auto build an excutor
6. Use the excutor 

***
*For example*
```csharp
public interface ITest : IWebApiInvoker<ITest>
{
    [HttpFullUrl("https://www.cnblogs.com/")]
    string TestFullUrl();

    [HttpFullUrl("https://www.cnblogs.com/")]
    Task<string> TestFullUrlAsync();
}
```
```csharp
[HttpHost("http://localhost:49386/")]
public interface IArgumentTest : IWebApiInvoker<IArgumentTest>
{
    [HttpUrl("/api/students")]
    string TestFormRequest1([ToBody(FormatType.Form)]Student student);
    [HttpUrl("/api/arg")]
    Foo TestFormRequest2([ToBody(FormatType.Form)]string arg1);
}
```
```csharp
var test = WebApiExecutionGenerator<ITest>.Create();
var response = test.TestFullUrl();
Console.WriteLine(response);
var response1 = await test.TestFullUrlAsync();
Console.WriteLine(response1);
```

Also you can inject the excutor if you have a `DI` module.


## Advanced Usage
If you have a string like `{name}` in `Http*Attribute` and also a argument have the same `name`,the value of the argument will replace the `{name}` placeholder.

**The `name` type must be value type**
```csharp
[HttpHost("http://localhost:49386/")]
public interface IArgumentTest : IWebApiInvoker<IArgumentTest>
{
    [HttpUrl("/api/students/{classRoom}")]
    string TestFormRequest1(string classRoom,[ToBody(FormatType.Form)]Student student);
}

public static void main()
{
    //the request url will be http://localhost:49386/api/students/c1
    WebApiExecutionGenerator<IArgumentTest>.Create().TestFormRequest1("c1",student);
}
```

If you use `ToQueryAttribute`, the argument will combine to the url as query string.

*For example*

```csharp
public class Student
{
    public string Name { get; set; }
    public int Age { get; set; }
}

[HttpHost("http://localhost:49386/")]
public interface IArgumentTest : IWebApiInvoker<IArgumentTest>
{
    [HttpUrl("/api/students/{classRoom}")]
    string TestFormRequest1(string classRoom,[ToQuery]Student student);
}

public static void main()
{
    var student = new Student
    {
        Name = "bob",
        Age = 20
    };
    //the request url will be http://localhost:49386/api/students/c1?Name=bob&Age=20
    WebApiExecutionGenerator<IArgumentTest>.Create().TestFormRequest1("c1",student);
}
```


# How to config
In every excutor, you have the `Config` method. That have an `HttpConfig` argument. Use this can config Timeout and RequestHeaders and Logger

```csharp
public interface IWebApiInvoker<ITarget>
{
    /// <summary>
    /// Config web request
    /// </summary>
    /// <param name="apiConfig"></param>
    /// <returns></returns>
    ITarget Config(HttpConfig apiConfig);
}

public class HttpConfig
{
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);
    public HttpRequestHeaders RequestHeaders { get; set; }
    public ILogger Logger { get; set; }
}
```

# Future
* DI extend package
* Polly in build
* Consul extend package
* Eureka extend package
* Etcd extend package
* Zookeeper extend package

# Thanks
This project was based on `DispatchProxyAsync` (I only have some little change), witch created by **gencebay** and **tahaipek**, very thanks to them.

>https://github.com/NetCoreStack/DispatchProxyAsync