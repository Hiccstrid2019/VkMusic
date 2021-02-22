using System;
using VkNet.Utils;
using VkNet.Model.Attachments;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkMusic
{
    class Playlist
    {
        public List<Song> Songs = new List<Song>();
        public Playlist(VkCollection<Audio> playlist, string titlePlaylist, long idPlaylist, string urlPhoto)
        {
            Title = titlePlaylist;
            Id = idPlaylist;
            PhotoUrl = urlPhoto;
            foreach (var song in playlist)
            {
                if (song.Url != null)
                    Songs.Add(new Song(song.Title, song.Artist, song.Duration, song.Url.ToString()));
            }
            CountSongs = Songs.Count;
        }

        public Song this[int i]
        {
            get
            {
                if (i >= 0 && i < CountSongs)
                    return Songs[i];
                if (i == -1)
                {
                    CurrentIndexSong = Convert.ToInt32(CountSongs) - 1;
                    return Songs[CurrentIndexSong];
                }    
                else
                {
                    CurrentIndexSong = 0;
                    return Songs[CurrentIndexSong];
                }
            }
        }
        public int CurrentIndexSong { get; set; } = 0;
        public string Title { get; set; }

        public string Author { get; set; }
        public long Id { get; set; }
        public long CountSongs { get; set; }
        public string PhotoUrl { get; set; }
    }
}
