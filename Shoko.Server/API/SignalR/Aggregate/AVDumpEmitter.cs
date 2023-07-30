using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Shoko.Plugin.Abstractions;
using Shoko.Server.API.SignalR.Models;

namespace Shoko.Server.API.SignalR.Aggregate;

public class AVDumpEmitter : BaseEmitter, IDisposable
{
    private IShokoEventHandler EventHandler { get; set; }

    public AVDumpEmitter(IHubContext<AggregateHub> hub, IShokoEventHandler events) : base(hub)
    {
        EventHandler = events;
        EventHandler.AVDumpMessage += OnAVDumpMessage;
    }

    public void Dispose()
    {
        EventHandler.AVDumpMessage -= OnAVDumpMessage;
    }

    private async void OnAVDumpMessage(object sender, AVDumpMessageEventArgs eventArgs)
    {
        await SendAsync("Message", new AVDumpMessageEventSignalRModel(eventArgs));
    }

    public override object GetInitialMessage()
    {
        return AVDumpHelper.GetActiveSessions()
            .Select(session => new AVDumpMessageEventSignalRModel(session.FilePath, session.VideoID, session.CommandID, session.StartedAt, session.Progress))
            .ToList();
    }
}
