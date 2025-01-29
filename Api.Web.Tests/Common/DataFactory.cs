using AutoFixture;

namespace Application.Tests.Common
{
    internal class DataFactory
    {


        public static string TestUrl(string path)
        {
            return new Uri(HttpClientTestBuilder.TestBaseUrl, path).AbsolutePath;
        }
    }
}
