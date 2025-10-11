namespace eShop.Application.Models
{
    public class ResponseWrapper : IResponseWrapper
    {
        public List<string> Messages { get; set; } = [];
        public bool IsSuccessful { get; set; }


        #region Fail Synchronously
        public static IResponseWrapper Fail()
        {
            return new ResponseWrapper() { IsSuccessful = false };
        }

        public static IResponseWrapper Fail(string messsage)
        {
            return new ResponseWrapper() { IsSuccessful = false, Messages = [messsage] };
        }

        public static IResponseWrapper Fail(List<string> messsages)
        {
            return new ResponseWrapper() { IsSuccessful = false, Messages = messsages };
        }
        #endregion

        #region Fail Asynchronously
        public static Task<IResponseWrapper> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        public static Task<IResponseWrapper> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }

        public static Task<IResponseWrapper> FailAsync(List<string> messages)
        {
            return Task.FromResult(Fail(messages));
        }
        #endregion

        #region Success Synchronously
        public static IResponseWrapper Success()
        {
            return new ResponseWrapper() { IsSuccessful = true };
        }

        public static IResponseWrapper Success(string messsage)
        {
            return new ResponseWrapper() { IsSuccessful = true, Messages = [messsage] };
        }

        public static IResponseWrapper Success(List<string> messsages)
        {
            return new ResponseWrapper() { IsSuccessful = true, Messages = messsages };
        }
        #endregion

        #region Success Asynchronously
        public static Task<IResponseWrapper> SuccessAsync()
        {
            return Task.FromResult(Success());
        }

        public static Task<IResponseWrapper> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }

        public static Task<IResponseWrapper> SuccessAsync(List<string> messages)
        {
            return Task.FromResult(Success(messages));
        }
        #endregion
    }

    public class ResponseWrapper<T> : ResponseWrapper, IResponseWrapper<T>
    {
        public T Data { get; set; }

        public ResponseWrapper() { }


        #region Fail Synchronously
        public new static ResponseWrapper<T> Fail()
        {
            return new ResponseWrapper<T>() { IsSuccessful = false };
        }

        public new static ResponseWrapper<T> Fail(string message)
        {
            return new ResponseWrapper<T>() { IsSuccessful = false, Messages = [message] };
        }

        public new static ResponseWrapper<T> Fail(List<string> messages)
        {
            return new ResponseWrapper<T>() { IsSuccessful = false, Messages = messages };
        }
        #endregion

        #region Fail Asynchronously
        public new static Task<ResponseWrapper<T>> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        public new static Task<ResponseWrapper<T>> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }

        public new static Task<ResponseWrapper<T>> FailAsync(List<string> messages)
        {
            return Task.FromResult(Fail(messages));
        }
        #endregion

        #region Success Synchronously
        public new static ResponseWrapper<T> Success()
        {
            return new ResponseWrapper<T>() { IsSuccessful = true };
        }

        public new static ResponseWrapper<T> Success(string message)
        {
            return new ResponseWrapper<T>() { IsSuccessful = true, Messages = [message] };
        }

        public new static ResponseWrapper<T> Success(List<string> messages)
        {
            return new ResponseWrapper<T>() { IsSuccessful = true, Messages = messages };
        }

        public static ResponseWrapper<T> Success(T data)
        {
            return new ResponseWrapper<T>() { Data = data, IsSuccessful = true };
        }

        public static ResponseWrapper<T> Success(T data, string message)
        {
            return new ResponseWrapper<T>() { Data = data, IsSuccessful = true, Messages = [message] };
        }

        public static ResponseWrapper<T> Success(T data, List<string> messages)
        {
            return new ResponseWrapper<T>() { Data = data, IsSuccessful = true, Messages = messages };
        }
        #endregion

        #region Success Asynchronously
        public new static Task<ResponseWrapper<T>> SuccessAsync()
        {
            return Task.FromResult(Success());
        }

        public new static Task<ResponseWrapper<T>> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }

        public new static Task<ResponseWrapper<T>> SuccessAsync(List<string> messages)
        {
            return Task.FromResult(Success(messages));
        }

        public static Task<ResponseWrapper<T>> SuccessAsync(T data)
        {
            return Task.FromResult(Success(data));
        }

        public static Task<ResponseWrapper<T>> SuccessAsync(T data, string message)
        {
            return Task.FromResult(Success(data, message));
        }

        public static Task<ResponseWrapper<T>> SuccessAsync(T data, List<string> messages)
        {
            return Task.FromResult(Success(data, messages));
        }
        #endregion
    }
}
