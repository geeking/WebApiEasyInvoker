using System;
using System.Threading.Tasks;
using WebApiEasyInvoker;
using WebApiEasyInvoker.Attributes;

namespace XUnitTestWebApiEasyInvoker
{
    [HttpHost("http://localhost:49386/")]
    public interface IReturnTypeTest : IWebApiInvoker<IReturnTypeTest>
    {
        [HttpUrl("api/values?ret=true")]
        bool RetBool();

        [HttpUrl("api/values?ret=true")]
        Task<bool> RetBoolAsync();

        [HttpUrl("api/values?ret=22")]
        byte RetByte();

        [HttpUrl("api/values?ret=22")]
        Task<byte> RetByteAsync();

        [HttpUrl("api/values?ret=s")]
        char RetChar();

        [HttpUrl("api/values?ret=s")]
        Task<char> RetCharAsync();

        [HttpUrl("api/values?ret=2020/1/1 8:00:00")]
        DateTime RetDateTime();

        [HttpUrl("api/values?ret=2020/1/1 8:00:00")]
        Task<DateTime> RetDateTimeAsync();

        [HttpUrl("api/values?ret=21324.42323")]
        decimal RetDecimal();

        [HttpUrl("api/values?ret=21324.42323")]
        Task<decimal> RetDecimalAsync();

        [HttpUrl("api/values?ret=123423.123423")]
        double RetDouble();

        [HttpUrl("api/values?ret=123423.123423")]
        Task<double> RetDoubleAsync();

        [HttpUrl("api/values?ret=-234")]
        short RetShort();

        [HttpUrl("api/values?ret=-234")]
        Task<short> RetShortAsync();

        [HttpUrl("api/values?ret=1234")]
        int RetInt();

        [HttpUrl("api/values?ret=1234")]
        Task<int> RetIntAsync();

        [HttpUrl("api/values?ret=342343422")]
        long RetLong();

        [HttpUrl("api/values?ret=342343422")]
        Task<long> RetLongAsync();

        [HttpUrl("api/values?ret=sfnldfjdsgawefsdgsdf")]
        string RetString();

        [HttpUrl("api/values?ret=sfnldfjdsgawefsdgsdf")]
        Task<string> RetStringAsync();
    }
}