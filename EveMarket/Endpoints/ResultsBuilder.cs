using System.Net;
using ErrorOr;

namespace Api.Web.Endpoints;

public static class ResultsBuilder
{
    public static IResult AsHttpResult<T>(this ErrorOr<T> result, Func<T, IResult>? success = null)
    {
        if (result.ErrorsOrEmptyList.Count > 1)
        {
            var validationErrors = result.ErrorsOrEmptyList
                .GroupBy(e => e.Code)
                .ToDictionary(eg => eg.Key, eg => eg.Select(e => e.Description).ToArray());

            return Results.ValidationProblem(validationErrors);
        }

        IResult GenerateProblem(Error error, HttpStatusCode statusCode)
        {
            return Results.Problem(title: error.Code, detail: error.Description, statusCode: (int)statusCode);
        }

        IResult GenerateValidationProblem(Error error)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [error.Code] = new[] { error.Description }
            }, title: error.Code, detail: error.Description);
        }

        return result.MatchFirst(success ?? GenerateProperSuccess, e => e.Type switch
        {
            ErrorType.Conflict => GenerateProblem(e, HttpStatusCode.Conflict),
            ErrorType.Failure => GenerateProblem(e, HttpStatusCode.InternalServerError),
            ErrorType.Unexpected => GenerateProblem(e, HttpStatusCode.InternalServerError),
            ErrorType.Validation => GenerateValidationProblem(e),
            ErrorType.NotFound => GenerateProblem(e, HttpStatusCode.NotFound),
            ErrorType.Unauthorized => GenerateProblem(e, HttpStatusCode.Unauthorized),
            _ => GenerateProblem(e, HttpStatusCode.InternalServerError),
        });
    }

    public static async Task<IResult> AsHttpResultAsync<T>(this Task<ErrorOr<T>> resulting, Func<T, IResult>? success = null)
    {
        var result = await resulting;
        return result.AsHttpResult(success);
    }

    private static IResult GenerateProperSuccess<T>(T response)
    {
        return response switch
        {
            Success => Results.Accepted(),
            Created => Results.Ok(),
            Deleted => Results.Accepted(),
            _ => Results.Ok(response)
        };
    }
}