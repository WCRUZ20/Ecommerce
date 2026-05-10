using System;
using System.Collections.Generic;
using System.Text;

namespace DealerEcommerce.Application.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }

        public string Message { get; private set; } = string.Empty;

        public T? Data { get; private set; }

        public static Result<T> Success(T data, string message = "Proceso realizado correctamente")
        {
            return new Result<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        public static Result<T> Failure(string message)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Message = message,
                Data = default
            };
        }
    }
}
