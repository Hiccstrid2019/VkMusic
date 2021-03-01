using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VkNet.Abstractions;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Threading;
using System.Security.Cryptography;
using System.Configuration;
using System.Collections.Specialized;

namespace VkMusic
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (Properties.Settings.Default.VkToken == "")
                api.Authorize(new ApiAuthParams { AccessToken = Properties.Settings.Default.VkToken });
            else
            {
                LoginWindow loginWindow = new LoginWindow();
                if (loginWindow.ShowDialog() == true)
                {

                    api.Authorize(new ApiAuthParams
                    {
                        Login = loginWindow.Login,
                        Password = loginWindow.Password,
                    });
                    Properties.Settings.Default.VkToken = api.Token;
                    Properties.Settings.Default.UserId = api.UserId.Value;
                    Properties.Settings.Default.Save();
                }
            }
            
            
            var audios = api.Audio.Get(new AudioGetParams { Count = 6000 });
            if (audios.Count != 0)
            {
                playlists.Add(new Playlist(audios, "Отдельные треки", 0, "Resources/music-album.png"));
                ExistDefaultPlaylist = true;
            }
            

            int startCountPlaylist = (ExistDefaultPlaylist) ? 8 : 9;
            DownloadPlaylist(startCountPlaylist);
            
            
        }
        private VkApi api = new VkApi(new ServiceCollection().AddAudioBypass());
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly MediaPlayer mediaPlayer = new MediaPlayer();
        

        List<Playlist> playlists = new List<Playlist>();
        private int Offset = 0;
        private bool ExistDefaultPlaylist = false;

        private bool IsPlaying;
        Playlist currentPlaylist;
        private void AddNewPlaylists(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer) sender;
            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                DownloadPlaylist(9);
            }
        }
        private void PlaySong()
        {
            Song currentSong = currentPlaylist[currentPlaylist.CurrentIndexSong];
            SongTitle.Text = currentSong.Title;
            SongArtist.Text = currentSong.Artist;
            SongTime.Text = TimeSpan.FromSeconds(currentSong.Time).ToString(@"mm\:ss");
            TimeLine.Minimum = 0;
            TimeLine.Maximum = currentSong.Time;
            mediaPlayer.Open(new Uri(currentSong.Url));
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;
            mediaPlayer.MediaEnded += SongEnding;
            timer.Start();
            IsPlaying = true;
            buttonPlay.Source = new BitmapImage(new Uri("Resources/pause.png", UriKind.Relative));
            mediaPlayer.Play();
        }
        private void playingPlaylist(object sender, RoutedEventArgs e)
        {
            currentPlaylist = playlists[Space.Children.IndexOf((StackPanel)sender)];
            currentPlaylist.CurrentIndexSong = 0;
            PlaySong();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            TimeLine.Value = mediaPlayer.Position.TotalSeconds;
            SongTime.Text = TimeSpan.FromSeconds(mediaPlayer.Position.TotalSeconds).ToString(@"mm\:ss");
        }
        private void PlayPause(object sender, MouseEventArgs e)
        {
            if (IsPlaying)
            {
                mediaPlayer.Pause();
                buttonPlay.Source = new BitmapImage(new Uri("Resources/play.png", UriKind.Relative));
                IsPlaying = false;
            }
            else
            {
                mediaPlayer.Play();
                buttonPlay.Source = new BitmapImage(new Uri("Resources/pause.png", UriKind.Relative));
                IsPlaying = true;
            }
        }
        private void SongEnding(object sender, EventArgs e)
        {
            currentPlaylist.CurrentIndexSong += 1;
            PlaySong();
        }

        private void PlaySong(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
        }
        private void PauseSong(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }
        private void BTClickPrev(object sender, MouseEventArgs e)
        {
            currentPlaylist.CurrentIndexSong -= 1;
            PlaySong();
        }
        private void BTClickNext(object sender, MouseEventArgs e)
        {
            currentPlaylist.CurrentIndexSong += 1;
            PlaySong();
        }
        private void ChangeVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = e.NewValue;
        }
        private void ChangePositionSong(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(e.NewValue);
        }
        
        private void DownloadPlaylist(int CountPlaylist)
        {
            var audioPlaylists = api.Audio.GetPlaylists(Properties.Settings.Default.UserId,
                                                        Convert.ToUInt32(CountPlaylist),
                                                        Convert.ToUInt32(Offset));
            foreach (var playlist in audioPlaylists)
            {
                if (playlist.Photo == null)
                    playlists.Add(new Playlist(api.Audio.Get(new AudioGetParams { PlaylistId = playlist.Id.Value }),
                                           playlist.Title, playlist.Id.Value,
                                           "Resources/music-album.png"));
                else
                    playlists.Add(new Playlist(api.Audio.Get(new AudioGetParams { PlaylistId = playlist.Id.Value }),
                                           playlist.Title, playlist.Id.Value,
                                           playlist.Photo.Photo135));
            }

            for (int i = (Offset > 0 && ExistDefaultPlaylist) ? Offset + 1: Offset; i < playlists.Count; i++)
            {
                
                Playlist plst = playlists[i];
                StackPanel currentPlaylist = new StackPanel()
                {
                    Height = 180
                };

                BitmapImage bIcon;
                try
                {
                    bIcon = new BitmapImage(new Uri(plst.PhotoUrl));
                }
                catch (UriFormatException)
                {
                    bIcon = new BitmapImage(new Uri(plst.PhotoUrl, UriKind.Relative));
                }
                System.Windows.Controls.Image iconPlaylist = new System.Windows.Controls.Image()
                {
                    Width = 135,
                    Height = 135,
                    Margin = new Thickness(5),
                    Source = bIcon
                };
                currentPlaylist.Children.Add(iconPlaylist);

                TextBlock titlePlaylist = new TextBlock()
                {
                    Width = 120,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    Text = plst.Title,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                currentPlaylist.Children.Add(titlePlaylist);

                currentPlaylist.MouseLeftButtonDown += playingPlaylist;

                Space.Children.Add(currentPlaylist);
            }
            Offset += CountPlaylist;
        }
    }

}
