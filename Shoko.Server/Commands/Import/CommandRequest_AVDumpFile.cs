using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shoko.Commons.Queue;
using Shoko.Models.Queue;
using Shoko.Models.Server;
using Shoko.Plugin.Abstractions;
using Shoko.Server.Commands.Attributes;
using Shoko.Server.Commands.Generic;
using Shoko.Server.Models;
using Shoko.Server.Repositories;
using Shoko.Server.Server;

namespace Shoko.Server.Commands;

[Serializable]
[Command(CommandRequestType.AVDumpFile)]
public class CommandRequest_AVDumpFile : CommandRequestImplementation
{
    public string FilePath { get; set; }

    public int VideoLocalID { get; set; }

    [XmlIgnore] [JsonIgnore]
    private SVR_VideoLocal Video;

    [XmlIgnore] [JsonIgnore]
    public string Result { get; protected set; } = string.Empty;

    public override CommandRequestPriority DefaultPriority => CommandRequestPriority.Priority3;

    public override QueueStateStruct PrettyDescription => new()
    {
        message = "AVDump File: {0}",
        queueState = QueueStateEnum.FileInfo,
        extraParams = new[] { FilePath },
    };

    [XmlIgnore] [JsonIgnore]
    private double Progress { get; set; } = 0;

    protected override void Process()
    {
        EventHandler<AVDumpMessageEventArgs> eventHandler = (_, eventArgs) =>
        {
            // Guard against concurrent dumps.
            if (eventArgs.VideoID != VideoLocalID || eventArgs.CommandID != CommandRequestID)
                return;
            switch (eventArgs.Type)
            {
                case AVDumpMessageType.Started:
                    OnStart();
                    break;
                case AVDumpMessageType.Progress:
                    OnProgressUpdate(eventArgs.Progress);
                    break;
                case AVDumpMessageType.Ended:
                case AVDumpMessageType.GenericException:
                    OnFinish();
                    break;
            }
        };

        try {
            ShokoEventHandler.Instance.AVDumpMessage += eventHandler;
            Result = AVDumpHelper.DumpFile(FilePath, Video, CommandRequestID);
        }
        finally
        {
            ShokoEventHandler.Instance.AVDumpMessage -= eventHandler;
        }
    }

    private void OnStart()
    {
        if (Processor == null)
            return;
        Processor.QueueState = new()
        {
            message = "AVDumping File: {0}",
            queueState = QueueStateEnum.FileInfo,
            extraParams = new[] { FilePath },
        };
    }

    private void OnProgressUpdate(double progress)
    {
        if (Processor == null)
            return;
        Processor.QueueState = new()
        {
            message = "AVDumping File: {0} — {1}%",
            queueState = QueueStateEnum.FileInfo,
            extraParams = new[] { FilePath, Math.Round(progress, 2).ToString() },
        };
    }

    private void OnFinish()
    {
        if (Processor == null)
            return;
        Processor.QueueState = new()
        {
            message = "AVDumped File: {0}",
            queueState = QueueStateEnum.FileInfo,
            extraParams = new[] { FilePath },
        };
    }

    public override void GenerateCommandID()
    {
        CommandID = $"CommandRequest_AVDumpFile_{VideoLocalID}";
    }

    public override bool LoadFromDBCommand(CommandRequest cq)
    {
        CommandID = cq.CommandID;
        CommandRequestID = cq.CommandRequestID;
        Priority = cq.Priority;
        CommandDetails = cq.CommandDetails;
        DateTimeUpdated = cq.DateTimeUpdated;

        // read xml to get parameters
        if (CommandDetails.Trim().Length <= 0) return false;

        var docCreator = new XmlDocument();
        docCreator.LoadXml(CommandDetails);

        // populate the fields
        FilePath = TryGetProperty(docCreator, "CommandRequest_AVDumpFile", "FilePath");
        VideoLocalID = int.Parse(TryGetProperty(docCreator, "CommandRequest_AVDumpFile", "VideoLocalID"));
        Video = RepoFactory.VideoLocal.GetByID(VideoLocalID);
        return true;
    }

    public override CommandRequest ToDatabaseObject()
    {
        GenerateCommandID();

        var cq = new CommandRequest
        {
            CommandID = CommandID,
            CommandType = CommandType,
            Priority = Priority,
            CommandDetails = ToXML(),
            DateTimeUpdated = DateTime.Now
        };
        return cq;
    }

    public CommandRequest_AVDumpFile(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }

    protected CommandRequest_AVDumpFile()
    {
    }
}
