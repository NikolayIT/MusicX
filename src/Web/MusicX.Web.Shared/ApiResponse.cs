namespace MusicX.Web.Shared
{
    using System.Collections.Generic;
    using System.Linq;

    public class ApiResponse<T>
    {
        public ApiResponse()
        {
        }

        public ApiResponse(T data)
        {
            this.Data = data;
        }

        public ApiResponse(IEnumerable<ApiError> errors)
        {
            if (errors == null || !errors.Any())
            {
                this.Errors = new List<ApiError> { new ApiError("ApiResponse", "Unspecified error.") };
            }
            else
            {
                this.Errors = errors;
            }
        }

        public ApiResponse(ApiError error)
        {
            this.Errors = new List<ApiError> { error };
        }

        public IEnumerable<ApiError> Errors { get; set; }

        public T Data { get; set; }

        public bool IsOk => !this.Errors?.Any() ?? true;
    }
}
