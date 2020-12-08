using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Cell.Blazor._Core.Class
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class UploaderLocale
    {
        public string Browse { get; set; } = "Browse...";

        public string Clear { get; set; } = nameof(Clear);

        public string Upload { get; set; } = nameof(Upload);

        [JsonPropertyName("dropFilesHint")]
        public string DropFilesHint { get; set; } = "Or drop files here";

        [JsonPropertyName("invalidMaxFileSize")]
        public string InvalidMaxFileSize { get; set; } = "File size is too large";

        [JsonPropertyName("invalidMinFileSize")]
        public string InvalidMinFileSize { get; set; } = "File size is too small";

        [JsonPropertyName("invalidFileType")]
        public string InvalidFileType { get; set; } = "File type is not allowed";

        [JsonPropertyName("uploadFailedMessage")]
        public string UploadFailedMessage { get; set; } = "File failed to upload";

        [JsonPropertyName("uploadSuccessMessage")]
        public string UploadSuccessMessage { get; set; } = "File uploaded successfully";

        [JsonPropertyName("removedSuccessMessage")]
        public string RemovedSuccessMessage { get; set; } = "File removed successfully";

        [JsonPropertyName("removedFailedMessage")]
        public string RemovedFailedMessage { get; set; } = "Unable to remove file";

        [JsonPropertyName("inProgress")]
        public string InProgress { get; set; } = "Uploading";

        [JsonPropertyName("readyToUploadMessage")]
        public string ReadyToUploadMessage { get; set; } = "Ready to upload";

        [JsonPropertyName("abort")]
        public string Abort { get; set; } = nameof(Abort);

        [JsonPropertyName("remove")]
        public string Remove { get; set; } = nameof(Remove);

        [JsonPropertyName("cancel")]
        public string Cancel { get; set; } = nameof(Cancel);

        [JsonPropertyName("delete")]
        public string Delete { get; set; } = "Delete file";

        [JsonPropertyName("pauseUpload")]
        public string PauseUpload { get; set; } = "File upload paused";

        [JsonPropertyName("pause")]
        public string Pause { get; set; } = nameof(Pause);

        [JsonPropertyName("resume")]
        public string Resume { get; set; } = nameof(Resume);

        [JsonPropertyName("retry")]
        public string Retry { get; set; } = nameof(Retry);

        [JsonPropertyName("fileUploadCancel")]
        public string FileUploadCancel { get; set; } = "File upload canceled";
    }
}