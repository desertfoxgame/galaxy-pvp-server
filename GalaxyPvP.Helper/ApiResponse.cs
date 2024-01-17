

namespace GalaxyPvP.Extensions
{
    public class ApiResponse<T>
    {
        private ApiResponse(int statusCode, List<string> errors)
        {
            StatusCode = statusCode;
            Errors = errors;
        }

        private ApiResponse(Exception ex)
        {
            Errors = new List<string> { ex.Message.ToString() };
        }

        private ApiResponse(int statusCode, T data)
        {
            StatusCode = statusCode;
            Data = data;
        }

        public bool Success
        {
            get
            {
                return Errors == null || Errors.Count == 0;
            }
        }

        public T Data { get; set; }
        public int StatusCode { get; set; } = 200;
        public List<string> Errors { get; set; } = new List<string>();

        public static ApiResponse<T> ReturnException(Exception ex)
        {
            return new ApiResponse<T>(ex);
        }

        public static ApiResponse<T> ReturnFailed(int statusCode, List<string> errors)
        {
            return new ApiResponse<T>(statusCode, errors);
        }

        public static ApiResponse<T> ReturnFailed(int statusCode, string errorMessage)
        {
            return new ApiResponse<T>(statusCode, new List<string> { errorMessage });
        }

        public static ApiResponse<T> ReturnSuccess()
        {
            return new ApiResponse<T>(200, null);
        }

        public static ApiResponse<T> ReturnResultWith200(T data)
        {
            return new ApiResponse<T>(200, data);
        }

        public static ApiResponse<T> ReturnResultWith201(T data)
        {
            return new ApiResponse<T>(201, data);
        }

        public static ApiResponse<T> ReturnResultWith204()
        {
            return new ApiResponse<T>(204, null);
        }

        public static ApiResponse<T> Return500()
        {
            return new ApiResponse<T>(500, new List<string> { "An unexpected fault happened. Try again later." });
        }

        public static ApiResponse<T> Return409(string message)
        {
            return new ApiResponse<T>(409, new List<string> { message });
        }
        public static ApiResponse<T> Return422(string message)
        {
            return new ApiResponse<T>(422, new List<string> { message });
        }

        public static ApiResponse<T> Return404()
        {
            return new ApiResponse<T>(404, new List<string> { "Not Found" });
        }

        public static ApiResponse<T> Return404(string message)
        {
            return new ApiResponse<T>(404, new List<string> { message });
        }
    }

}
