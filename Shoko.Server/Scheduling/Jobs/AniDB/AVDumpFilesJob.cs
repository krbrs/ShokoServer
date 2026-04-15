using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shoko.Server.Scheduling.Acquisition.Attributes;
using Shoko.Server.Scheduling.Attributes;
using Shoko.Server.Scheduling.Concurrency;
using Shoko.Server.Utilities;

namespace Shoko.Server.Scheduling.Jobs.AniDB;

[DatabaseRequired]
[NetworkRequired]
[LimitConcurrency(1, 16)]
public class AVDumpFilesJob : BaseJob<AVDumpHelper.AVDumpSession>
{
    /// <summary>
    /// Videos to dump.
    /// </summary>
    public Dictionary<int, string> Videos { get; set; }

    /// <summary>
    /// Hash key representing the videos to dump.
    /// </summary>
    [JobKeyMember]
    public string Key
    {
        get => Videos is not null
            ? Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Videos.OrderBy(a => (a.Key, a.Value)).ToDictionary()))))
            : string.Empty;
        set { }
    }

    public override string Title => "AVDumping Files";
    public override string TypeName => "AVDump Files";
    public override Dictionary<string, object> Details =>
        Videos.Values.Select((value, index) => (index, value)).ToDictionary(a => a.index.ToString(), a => (object)a.value);

    public override Task<AVDumpHelper.AVDumpSession> Process()
    {
        var session = AVDumpHelper.DumpFiles(Videos, synchronous: true);

        return Task.FromResult(session);
    }
}
