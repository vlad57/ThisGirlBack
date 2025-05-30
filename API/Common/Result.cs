using System;
using API.Models;

namespace API.Common;

public class Result<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public int? Code { get; set; }

    public static Result<T> Ok(T data) => new() { Success = true, Data = data };
    public static Result<T> Fail(string error, int? code) => new() { Success = false, Error = error, Code = code };
}
