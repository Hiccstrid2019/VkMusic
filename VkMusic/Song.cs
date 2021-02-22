using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkMusic
{
    class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public double Time { get; set; }
        public string TimeString { get; set; }
        public string Url { get; set; }
        public string OriginalUrl { get; set; }

        public Song(string title, string artist, double time, string originalUrl)
        {
            Title = title;
            Artist = artist;
            Time = time;
            TimeString = TimeSpan.FromSeconds(time).ToString(@"mm\:ss");
            OriginalUrl = originalUrl;
            Url = extractMP3fromUrl(originalUrl);
        }
        private string extractMP3fromUrl(string audioUrl)
        {
            audioUrl = audioUrl.Split('?')[0];
            if (audioUrl.Contains(".mp3"))
                return audioUrl.Replace("https", "http");
            string[] chunksUrl = audioUrl.Split('/');
            if (audioUrl.Contains("audios"))
            {
                audioUrl = $"{chunksUrl[0]}//{chunksUrl[2]}/{chunksUrl[3]}/{chunksUrl[4]}/{chunksUrl[6]}/{chunksUrl[7]}.mp3".Replace("https", "http");
                return audioUrl;
            }
            audioUrl = $"{chunksUrl[0]}//{chunksUrl[2]}/{chunksUrl[3]}/{chunksUrl[5]}.mp3".Replace("https", "http");
            return audioUrl;
        }
    }
}
