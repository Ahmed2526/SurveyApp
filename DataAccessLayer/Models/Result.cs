namespace DataAccessLayer.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public int StatusCode { get; }
        public string[]? Errors { get; }
        public T? Data { get; }

        private Result(bool success, int statuscode, T? data, string[]? errors)
        {
            IsSuccess = success;
            StatusCode = statuscode;
            Data = data;
            Errors = errors;
        }

        public static Result<T> Success(int statuscode, T data)
        {
            return new Result<T>(success: true, statuscode: statuscode, data: data, errors: null);
        }

        public static Result<T> Fail(int statuscode, string[] errors)
        {
            return new Result<T>(success: false, statuscode: statuscode, data: default, errors: errors);
        }

    }
}
