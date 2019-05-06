using System;

namespace DownloaderImagesModels
{
    internal class DownloadEventArgs : EventArgs
    {
        internal bool Process { get; set; }
        internal string Hour { get; set; }
    }
}