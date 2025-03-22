using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace CQRSPattern.Shared.Test;

public class MockedHttpClient
{
    /// <summary>
    /// Get an httpclient which has its httpmessagehandler mocked to simulate api calls.
    /// </summary>
    /// <typeparam name="T">Type of object to be returned by the httpclient.</typeparam>
    /// <param name="mockedHandler">To verify messages</param>
    /// <param name="objectToBeReturnedByApi">Object which shall be returned by the httpclient.</param>
    /// <returns></returns>
    public HttpClient GetMockedHttpClient<T>(out Mock<HttpMessageHandler> mockedHandler, T objectToBeReturnedByApi)
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Default);
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                )
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(objectToBeReturnedByApi))
            })
            .Verifiable();
        var client = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("https://omp.unit.test") };

        mockedHandler = handlerMock;
        return client;
    }
}
