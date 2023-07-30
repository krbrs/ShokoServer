using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Shoko.Plugin.Abstractions;

#nullable enable
namespace Shoko.Server.API.SignalR.Models;

public class AVDumpMessageEventSignalRModel
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string FilePath { get; set; }

    public int VideoID { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? CommandID { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public AVDumpMessageType Type { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public double? Progress { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Message { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? ErrorMessage { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? ExceptionStackTrace { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? StartedAt { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? SentAt { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? EndedAt { get; set; }

    public AVDumpMessageEventSignalRModel(AVDumpMessageEventArgs eventArgs)
    {
        FilePath = eventArgs.FilePath;
        VideoID = eventArgs.VideoID;
        CommandID = eventArgs.CommandID;
        Type = eventArgs.Type;
        Progress = eventArgs.Progress;
        Message = eventArgs.Message;
        ErrorMessage = eventArgs.ErrorMessage;
        ExceptionStackTrace = eventArgs.Exception?.StackTrace;
        StartedAt = eventArgs.StartedAt;
        SentAt = eventArgs.SentAt;
        EndedAt = eventArgs.EndedAt;
    }

    public AVDumpMessageEventSignalRModel(string filePath, int videoId, int? commandId, DateTime startedAt, double progress)
    {
        FilePath = filePath;
        VideoID = videoId;
        CommandID = commandId;
        Type = AVDumpMessageType.Running;
        Progress = progress;
        StartedAt = startedAt;
        SentAt = DateTime.UtcNow;
    }
}
