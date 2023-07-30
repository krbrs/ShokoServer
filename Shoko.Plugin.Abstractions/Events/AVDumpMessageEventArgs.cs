
using System;

namespace Shoko.Plugin.Abstractions
{
    public class AVDumpMessageEventArgs : EventArgs
    {
        public string FilePath { get; }

        public int VideoID { get; }

        public int? CommandID { get; }

        public AVDumpMessageType Type { get; }

        public double? Progress { get; }

        public bool? Success { get; }

        public string Message { get; }

        public string ErrorMessage { get; }

        public Exception Exception { get; }

        public DateTime? StartedAt { get; }

        public DateTime? SentAt { get; }

        public DateTime? EndedAt { get; }

        public AVDumpMessageEventArgs(AVDumpMessageType messageType, string message = null)
        {
            VideoID = 0;
            Type = messageType;
            Message = message;
        }

        public AVDumpMessageEventArgs(string filePath, int videoId, int? commandId, DateTime startedAt)
        {
            FilePath = filePath;
            VideoID = videoId;
            CommandID = commandId;
            Type = AVDumpMessageType.Started;
            Progress = 0;
            StartedAt = startedAt;
        }

        public AVDumpMessageEventArgs(string filePath, int videoId, int? commandId, AVDumpMessageType messageType, string message, double? progress = null)
        {
            FilePath = filePath;
            VideoID = videoId;
            CommandID = commandId;
            Type = messageType;
            Progress = progress;
            Message = message;
            SentAt = DateTime.UtcNow;
        }

        public AVDumpMessageEventArgs(string filePath, int videoId, int? commandId, DateTime startedAt, DateTime endedAt, bool success, string message, string errorMessage)
        {
            FilePath = filePath;
            VideoID = videoId;
            CommandID = commandId;
            Type = AVDumpMessageType.Ended;
            Progress = 100;
            Success = success;
            Message = message;
            if (!string.IsNullOrWhiteSpace(errorMessage))
                ErrorMessage = errorMessage;
            StartedAt = startedAt;
            EndedAt = endedAt;
        }

        public AVDumpMessageEventArgs(string filePath, int videoId, int? commandId, DateTime startedAt, Exception ex)
        {
            FilePath = filePath;
            VideoID = videoId;
            CommandID = commandId;
            Type = AVDumpMessageType.GenericException;
            Progress = 100;
            Message = ex.Message;
            Exception = ex;
            StartedAt = startedAt;
            EndedAt = DateTime.UtcNow;
        }

        public AVDumpMessageEventArgs(Exception ex)
        {
            VideoID = 0;
            Type = AVDumpMessageType.InstallException;
            Message = ex.Message;
            Exception = ex;
        }
    }

    public enum AVDumpMessageType
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
        /// A message indicating an AVDump session have ended for a given file
        /// and/or command request.
        /// </summary>
        Ended,

        /// <summary>
        /// A message only sent from th
        /// </summary>
        Running,

        /// <summary>
        /// A generic .NET exception occured while trying to run the AVDump
        /// session and the session have ended as a result.
        /// </summary>
        GenericException,

        /// <summary>
        /// A generic .NET exception occured while trying to run the AVDump
        /// session and the session have ended as a result.
        /// </summary>
        InstallException,

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
        /// The UDP AVDump Api Key is missing from the settings.
        /// </summary>
        MissingApiKey,

        /// <summary>
        /// Unable to authenticate with Anidb.
        /// </summary>
        InvalidCredentials,
    }
}
