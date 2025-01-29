using System.Net;
using ErrorOr;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

using Api.Web.Endpoints;


public class ResultsBuilderTests
{
    [TestCase(ErrorType.Failure, HttpStatusCode.InternalServerError)]
    [TestCase(ErrorType.Unexpected, HttpStatusCode.InternalServerError)]
    [TestCase(ErrorType.Validation, HttpStatusCode.BadRequest)]
    [TestCase(ErrorType.Conflict, HttpStatusCode.Conflict)]
    [TestCase(ErrorType.NotFound, HttpStatusCode.NotFound)]
    [TestCase(ErrorType.Unauthorized, HttpStatusCode.Unauthorized)]
    public void GivenKnownError_WhenConvertingToHttpResult_ThenReturnsExpectedHttpStatusCode(ErrorType type, HttpStatusCode expectedHttpStatusCode)
    {
        ErrorOr<string> error = Error.Custom((int)type, "TestCode", "TestDescription");
        var result = error.AsHttpResult();

        result.Should().BeAssignableTo<IStatusCodeHttpResult>().Which.StatusCode.Should().Be((int)expectedHttpStatusCode);
    }

    [TestCase(ErrorType.Failure)]
    [TestCase(ErrorType.Unexpected)]
    [TestCase(ErrorType.Validation)]
    [TestCase(ErrorType.Conflict)]
    [TestCase(ErrorType.NotFound)]
    [TestCase(ErrorType.Unauthorized)]
    public void GivenKnownError_WhenConvertingToHttpResult_ThenReturnsProblemPayload(ErrorType type)
    {
        ErrorOr<string> error = Error.Custom((int)type, "TestCode", "TestDescription");
        var result = error.AsHttpResult();

        result.Should().BeOfType<ProblemHttpResult>();
    }
    [Test]
    public void GivenKnownErrorList_WhenConvertingToHttpResult_ThenReturnsProblemPayload()
    {
        ErrorOr<string> error;
        List<Error> errors =
        [
            Error.Custom((int)ErrorType.Unauthorized, "TestCode", "TestDescription"),
        Error.Custom((int)ErrorType.Validation, "TestCode", "TestDescription")

        ];

        error = errors;

        var result = error.AsHttpResult();

        result.Should().BeOfType<ProblemHttpResult>();
    }
}