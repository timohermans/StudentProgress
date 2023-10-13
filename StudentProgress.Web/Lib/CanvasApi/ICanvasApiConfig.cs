﻿using System.Threading.Tasks;

namespace StudentProgress.Web.Lib.CanvasApi;

public interface ICanvasApiConfig
{
    string? CanvasApiKey { get; }
    string? CanvasApiUrl { get; }
    Task<bool> CanUseCanvasApiAsync();
}