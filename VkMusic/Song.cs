using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkMusic
{
    public class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public double Time { get; set; }
        public string TimeString { get; set; }
        public string Url { get; set; }
        public string OriginalUrl { get; set; }
    }
}
