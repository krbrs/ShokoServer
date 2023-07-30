
using System;

namespace Shoko.Plugin.Abstractions
{
    public class AVDumpEventArgs : EventArgs
    {
        /// <summary>
        /// Absolute path of file being dumped.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The video id, if applicable to the event type.
        /// </summary>
        /// <value></value>
        public int? VideoID { get; }

        /// <summary>
        /// The command request id, if applicable to the event type.
        /// </summary>
        /// <value></value>
        public int? CommandID { get; }

        /// <summary>
        /// The avdump event type. This is the most important property of the
        /// event, because it tells the event consumer which properties will be
        /// available.
        /// </summary>
        public AVDumpEventType Type { get; }

        /// <summary>
        /// The progress, this should be updated if it's sent with an event.
        /// </summary>
        public double? Progress { get; }

        /// <summary>
        /// The message for the event, if applicable.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// If a failure event occurs, then this property will contain the
        /// standard error.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// The exception, if an install or generic exception event occurs.
        /// </summary>
        /// <value></value>
        public Exception Exception { get; }

        /// <summary>
        /// When the AVDump session was started. Only sent in start, running and
        /// ending events.
        /// </summary>
        public DateTime? StartedAt { get; }

        /// <summary>
        /// When the message event was sent, only sent if it's applicable to the
        /// event.
        /// </summary>
        public DateTime? SentAt { get; }

        /// <summary>
        /// When the AVDump session ended. Only sent in ending events.
        /// </summary>
        public DateTime? EndedAt { get; }

        public AVDumpEventArgs(AVDumpEventType messageType, string message = null)
        {
            Type = messageType;
            Message = message;
        }

        public AVDumpEventArgs(string filePath, int videoId, int? commandId, DateTime startedAt)
        {
            Path = filePath;
            VideoID = videoId;
            CommandID = commandId;
            Type = AVDumpEventType.Started;
            Progress = 0;
            StartedAt = startedAt;
        }

        public AVDumpEventArgs(int videoId, int? commandId, AVDumpEventType messageType, string message, double? progress = null)
        {
            VideoID = videoId;
            CommandID = commandId;
            Type = messageType;
            Progress = progress;
            Message = message;
            SentAt = DateTime.UtcNow;
        }

        public AVDumpEventArgs(string filePath, int videoId, int? commandId, DateTime startedAt, DateTime endedAt, bool success, string message, string errorMessage)
        {
            Path = filePath;
            VideoID = videoId;
            CommandID = commandId;
            Type = success ? AVDumpEventType.Success : AVDumpEventType.Failure;
            Progress = 100;
            Message = message;
            ErrorMessage = errorMessage;
            StartedAt = startedAt;
            EndedAt = endedAt;
        }

        public AVDumpEventArgs(string filePath, int videoId, int? commandId, DateTime startedAt, Exception ex)
        {
            Path = filePath;
            VideoID = videoId;
            CommandID = commandId;
            Type = AVDumpEventType.GenericException;
            Progress = 100;
            Message = ex.Message;
            Exception = ex;
            StartedAt = startedAt;
            EndedAt = DateTime.UtcNow;
        }

        public AVDumpEventArgs(Exception ex)
        {
            Type = AVDumpEventType.InstallException;
            Message = ex.Message;
            Exception = ex;
        }
    }

    public enum AVDumpEventType
    {
        /// <summary>
        /// Any message sent to the standard output from the avdump binary that's
        /// not a progress message.
        /// </summary>
        Message = 0,

        /// <summary>
        /// Any message sent to the standard error from the avdump binary.
        /// </summary>
        Error,

        /// <summary>
        /// A progress update sent to the standard output from the avdump
        /// binary. Will contain an updated progress.
        /// </summary>
        Progress,

        /// <summary>
        /// A message indicating an AVDump session have started for a given
        /// file and/or command request.
        /// </summary>
        Started,

        /// <summary>
        /// A message indicating an AVDump session have ended with a success for
        /// a given file and/or command request.
        /// </summary>
        Success,

        /// <summary>
        /// A message indicating an AVDump session have ended with a failure for
        /// a given file and/or command request.
        /// </summary>
        Failure,

        /// <summary>
        /// A message sent for any running sessions to new SignalR clients.
        /// </summary>
        Running,

        /// <summary>
        /// A generic .NET exception occured while trying to run the AVDump
        /// session and the session have ended as a result.
        /// </summary>
        GenericException,

        /// <summary>
        /// The UDP AVDump Api Key is missing from the settings.
        /// </summary>
        MissingApiKey,

        /// <summary>
        /// Unable to authenticate with Anidb.
        /// </summary>
        InvalidCredentials,

        /// <summary>
        /// A message indicating we're trying to install AVDump before starting
        /// the AVDump session.
        /// </summary>
        InstallingAVDump,

        /// <summary>
        /// A message indicating we're trying to install AVDump before starting
        /// the AVDump session.
        /// </summary>
        InstalledAVDump,

        /// <summary>
        /// A generic .NET exception occured while trying to run the AVDump
        /// session and the session have ended as a result.
        /// </summary>
        InstallException,
    }
}
