using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApiEasyInvoker;
using WebApiEasyInvoker.Attributes;
using Xunit;

namespace XUnitTestWebApiEasyInvoker
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1Async()
        {
            var md = typeof(TestClass).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.Name == "InvokeAsync" && m.ReturnType == typeof(Task<>));//.FirstOrDefault();
            //var m = typeof(TestClass).GetMethod("InvokeAsync", BindingFlags.Instance | BindingFlags.NonPublic, null, CallingConventions.Any, new Type[] { typeof(MethodInfo), typeof(object[]) }, null);
            //m.Invoke(new TestClass(), new object[] { null, null });

            //var response1 = await WebApiExecutionGenerator<ITest>.Create().TestFullUrlAsync();

            var response = WebApiExecutionGenerator.Create<ITest>().TestFullUrl();
            Console.WriteLine(response);
            Assert.False(string.IsNullOrEmpty(response));
        }

        [Fact]
        public async void TestReturnType()
        {
            var retTest = WebApiExecutionGenerator.Create<IReturnTypeTest>();

            var a0 = retTest.RetBool();
            var a1 = retTest.RetByte();
            var a2 = retTest.RetChar();
            var a3 = retTest.RetDateTime();
            var a4 = retTest.RetDecimal();
            var a5 = retTest.RetDouble();
            var a6 = retTest.RetShort();
            var a7 = retTest.RetInt();
            var a8 = retTest.RetLong();
            var a9 = retTest.RetString();

            var b0 = await retTest.RetBoolAsync();
            var b1 = await retTest.RetByteAsync();
            var b2 = await retTest.RetCharAsync();
            var b3 = await retTest.RetDateTimeAsync();
            var b4 = await retTest.RetDecimalAsync();
            var b5 = await retTest.RetDoubleAsync();
            var b6 = await retTest.RetShortAsync();
            var b7 = await retTest.RetIntAsync();
            var b8 = await retTest.RetLongAsync();
            var b9 = await retTest.RetStringAsync();
        }

        [Fact]
        public void TestToBodyForm1()
        {

            var argTest = WebApiExecutionGenerator.Create<IArgumentTest>();
            var a1 = new Student { Name = "test", Age = 12 };
            argTest.TestFormRequest1(a1);
        }

        [Fact]
        public void TestToBodyForm2()
        {
            var argTest = WebApiExecutionGenerator.Create<IArgumentTest>();
            argTest.TestFormRequest2("foo");
        }
    }

    [HttpHost("http://localhost:49386/")]
    public interface IArgumentTest : IWebApiInvoker<IArgumentTest>
    {
        [HttpUrl("/api/students")]
        string TestFormRequest1([ToBody(FormatType.Form)]Student student);
        [HttpUrl("/api/arg")]
        string TestFormRequest2([ToBody(FormatType.Form)]string arg1);
    }

    public class Student
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
    public class ClassRoom
    {
        public int Level { get; set; }
        public string Name { get; set; }
    }
    public interface ITest : IWebApiInvoker<ITest>
    {
        [HttpFullUrl("https://www.cnblogs.com/")]
        string TestFullUrl();

        [HttpFullUrl("https://www.cnblogs.com/")]
        Task<string> TestFullUrlAsync();
    }

    public class TestClass
    {
        protected T Method1<T>(object obj)
        {
            return default(T);
        }

        protected Task InvokeAsync(MethodInfo targetMethod, object[] args)
        {
            Console.WriteLine("I");
            return Task.CompletedTask;
        }

        protected Task<T> InvokeAsync<T>(MethodInfo targetMethod, object[] args)
        {
            Console.WriteLine("T");
            return Task.FromResult<T>(default);
        }
    }
}