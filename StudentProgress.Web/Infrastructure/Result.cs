using System;

namespace StudentProgress.Web.Infrastructure;

public abstract class Result
{
    public bool IsSuccess { get; protected init; }
    public bool IsFailure => !IsSuccess;
}

public abstract class Result<T> : Result
{
    private T? _data;

    public T Data
    {
        get => IsSuccess
            ? _data ?? throw new Exception($"Use {nameof(Result<T>)} when you have data to return and then return it! Otherwise use the ${nameof(Result)} class.")
            : throw new Exception($"You can't access ${nameof(Data)} when ${nameof(IsSuccess)} is false");
        init => _data = value;
    }

    protected Result(T? data)
    {
        Data = data;
    }
}

public class SuccessResult : Result
{
    public SuccessResult()
    {
        IsSuccess = true;
    }
}

public class SuccessResult<T> : Result<T>
{
    public SuccessResult(T? data) : base(data)
    {
        IsSuccess = true;
    }
}

public class ErrorResult : Result
{
    public string Message { get; set; }

    public ErrorResult(string message)
    {
        Message = message;
    }
}

public class ErrorResult<T> : Result<T>
{
    public string Message { get; set; }

    public ErrorResult(string message) : base(default)
    {
        Message = message;
    }
}