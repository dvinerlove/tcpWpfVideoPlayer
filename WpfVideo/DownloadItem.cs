using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UrlExtractor.ServiceContracts;
using Vimeo1.UrlExtractor;
using System.Net;
using System.Threading;

namespace WpfVideo
{

    internal class DownloaderInput
    {
        public string FileName { get; set; }
        public string Url { get; set; }
        public CookieContainer CookieContainer { get; set; }
    }

    public enum DownloadStatus
    {
        Idle,
        Downloading,
        Completed,
        Paused,
        Cancelled
    };

    public partial class DownloadItem
    {
        //Each thread downloads a segment of the file. This dictionary stores the progress of each chunk
        private Dictionary<int, int> ChunkProgress = new Dictionary<int, int>();
        DownloaderInput downloaderInput1;
        public string SourceUrl { get; set; }
        //public DownloadStatus Status { get; set; } = DownloadStatus.Idle;
        //private IAsyncFileDownloader fileDownloader;
        //private CancellationTokenSource cancellationSource;
        public string Link{ get; set; }

        public  DownloadItem()
        {
            GetStreamUrl();
        }

        
        //InitializeComponent();
    
        public string GetStreamUrl()
        {
            if (downloaderInput1!= null)
            {
                return Link;
            }
            return "";
        }
        public async void Start()
        {
            // await ResolveDownloadUrl();

            await App.Current.Dispatcher.Invoke(async () =>
             {
                 Link = await ResolveDownloadUrl();
             });

            //App.Current.Dispatcher.Invoke(() => {
            //    Link = downloaderInput1.Url;
            //});
            //MessageBox.Show(downloaderInput1.Url+" start");
        }

        

        private async Task<String> ResolveDownloadUrl()
        {
            await Task.Delay(100);
            //resolve url extractor based on the URL hostname
            var extractor = ResolveExtractor();
            if (extractor != null)
            {
                //Extract the video urls and their metadata
                var videoInfo = await extractor.GetDownloadUrlsAsync(SourceUrl);


                //Ask for the quality of the video to be downloaded
                //VideoQualitySelector qualitySelector = new VideoQualitySelector();
                //qualitySelector.Text = videoInfo.Title;
                // var bq = videoInfo.DownloadUrls.Where(x => x.Quality == "1080p").FirstOrDefault().Quality;//.Select(x => x.Quality).FirstOrDefault();
                var bq1 = "";
                if (!string.IsNullOrWhiteSpace(videoInfo.DownloadUrls.Where(x => x.Quality == "1080p").FirstOrDefault().Url))
                {
                     bq1 = videoInfo.DownloadUrls.Where(x => x.Quality == "1080p").FirstOrDefault().Url;

                }
                else if(!string.IsNullOrWhiteSpace(videoInfo.DownloadUrls.Where(x => x.Quality == "720p").FirstOrDefault().Url))
                {
                     bq1 = videoInfo.DownloadUrls.Where(x => x.Quality == "720p").FirstOrDefault().Url;

                }
                else if (!string.IsNullOrWhiteSpace(videoInfo.DownloadUrls.Where(x => x.Quality == "540p").FirstOrDefault().Url))
                {
                     bq1 = videoInfo.DownloadUrls.Where(x => x.Quality == "540p").FirstOrDefault().Url;

                }
                else if (!string.IsNullOrWhiteSpace(videoInfo.DownloadUrls.Where(x => x.Quality == "360p").FirstOrDefault().Url))
                {
                     bq1 = videoInfo.DownloadUrls.Where(x => x.Quality == "360p").FirstOrDefault().Url;

                }
                else
                {
                     bq1 = videoInfo.DownloadUrls.Where(x => x.Quality == "240p").FirstOrDefault().Url;

                }
                    Link = bq1;
                
                //MessageBox.Show(bq + " " + bq1);
                //qualitySelector.QualityLabels = videoInfo.DownloadUrls.Select(x => x.Quality).ToList();
                //qualitySelector.ShowDialog();

                //return new 
                App.Current.Dispatcher.Invoke(() => {
                    Link = bq1; 
                });
                return bq1;
                //return the video url
                //return new DownloaderInput()
                //{
                //    Url = videoInfo.DownloadUrls.Where(x => x.Quality == bq).First().Url,
                //    FileName = videoInfo.Title + bq + ".mp4",
                //    CookieContainer = videoInfo.AuthCookieContainer
                //};

            }

            return null;
        }

        private IUrlExtractor ResolveExtractor()
        {
            Uri myUri = new Uri(SourceUrl);
            string host = myUri.Host;

            if (host.Contains("vimeo.com"))
            {
                return new VimeoExtractor();
            }
            else
                return null;
        }
    }
}