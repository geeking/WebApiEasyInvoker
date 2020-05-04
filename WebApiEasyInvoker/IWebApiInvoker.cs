using WebApiEasyInvoker.Config;

namespace WebApiEasyInvoker
{
    /// <summary>
    /// web api base interface for auto invoke
    /// </summary>
    /// <typeparam name="ITarget">the interface be proxyed</typeparam>
    public interface IWebApiInvoker<ITarget>
    {
        /// <summary>
        /// Config web request
        /// </summary>
        /// <param name="apiConfig"></param>
        /// <returns></returns>
        ITarget Config(HttpConfig apiConfig);
    }
}