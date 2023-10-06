using System.Text.Json.Serialization;

namespace Tribitgroup.Framework.Shared.Types
{
    public class Response<T> where T : class
    {
        public bool Success { get; set; }
        public int ResponseCode { get; set; } = 200;
        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public ExceptionWithCode? Error { get; set; } = null;
        public object Exception
        {
            get
            {
                if (Error == null)
                    return null;
                else
                    return new
                    {
                        Error.Message,
                        Error.Code
                    };
            }
        }
        public virtual T? Result { get; set; } = null;

        public Response()
        {

        }

        public Response(T? result)
        {
            Success = true;
            Result = result;
        }

        public static Response<object> SuccessResponse() => new(null);
        //public static IActionResult SuccessActionResult() => new OkResult();
        public static Response<T> SuccessResponse(T result) => new(result);
        //public static IActionResult SuccessActionResult(T result) => new OkObjectResult(result);
        public static Response<T> ErrorResponse(string code = "__UNKNOWN__", string error = "") => new()
        {
            Error = new ExceptionWithCode(code, error),
            Result = null,
            Success = false,
            ResponseCode = 400
        };
        //public static IActionResult ErrorActionResult(string code = "__UNKNOWN__", string error = "")
        //    => new BadRequestObjectResult(ErrorResponse(code, error));
        public static Response<object> ToResponse(Exception ex) => new()
        {
            Error = ex is ExceptionWithCode ? ex as ExceptionWithCode : new ExceptionWithCode($"__GENERAL__:{ex.HResult}"),
            Result = null,
            Success = false,
            ResponseCode = 400
        };

        //public static IActionResult ToActionResult(Exception ex)
        //    => new BadRequestObjectResult(ToResponse(ex));

        //public static IActionResult CreatedActionResult(string url, object result)
        //    => new CreatedResult(url, result);
    }

    public class Response : Response<object>
    {
        public override object Result => Success;
    }
}