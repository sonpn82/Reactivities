using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Core
{
    // Class to return a Result when using get request to get an Activity (before we only return an Activity from this request)
    // With Result.Success, the return object can be either an Acvitity or a null
    public class Result<T>  // T is generic type
    {
        public bool IsSuccess { get; set; }   

        public T? Value { get; set; }  // value will be type of T  ()

        public string? Error { get; set; }

        public static Result<T> Success(T value) => new Result<T> {IsSuccess = true, Value = value};
        public static Result<T> Failure(string error) => new Result<T> {IsSuccess = false, Error = error};
    }
}