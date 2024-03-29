﻿using CSharpFunctionalExtensions;

namespace StudentProgress.Core.Entities;

public class Setting : AuditableEntity<int>
{
    public string Key { get; private set; }
    public string Value { get; private set; }

    #nullable disable
    private Setting()
    {
    } 
    #nullable enable

    public Setting(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public Result Update(string requestCanvasApiKey)
    {
        Value = requestCanvasApiKey;
        return Result.Success();
    }

    public static class Keys
    {
        public const string CanvasApiKey = "canvasApiKey";
        public const string CanvasApiUrl = "canvasApiUrl";
    }
}