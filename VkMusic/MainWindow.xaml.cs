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

            List<Song> VKSongs = new List<Song>();
            var services = new ServiceCollection();
            services.AddAudioBypass();

            var api = new VkApi(services);

            try
            {
                if (Properties.Settings.Default.VkToken != "")
                    api.Authorize(new ApiAuthParams { AccessToken = Properties.Settings.Default.VkToken });
                else
                {
                    LoginWindow loginWindow = new LoginWindow();
                    if (loginWindow.ShowDialog() == true)
                    {
                        api.Authorize(new ApiAuthParams
                        {
                            Login = loginWindow.Login,
                            Password = loginWindow.Password
                        });
                        Properties.Settings.Default.VkToken = api.Token;
                        Properties.Settings.Default.UserId = api.UserId.Value;
                        Properties.Settings.Default.Save();
                    }
                }
            }
            catch (VkNet.AudioBypassService.Exceptions.VkAuthException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (VkNet.Exception.CaptchaNeededException e)
            {
                throw new Exception(e.Message);
            }
            
            var audios = api.Audio.Get(new AudioGetParams { Count = 6000 });
            foreach (var audio in audios)
            {
                TimeSpan ts = TimeSpan.FromSeconds(audio.Duration);
                VKSongs.Add(new Song
                {
                    Artist = audio.Artist,
                    Title = audio.Title,
                    Time = audio.Duration,
                    TimeString = ts.ToString(@"mm\:ss"),
                    Url = extractMP3fromUrl(audio.Url.ToString()),
                    OriginalUrl = audio.Url.ToString()
                });
            }

            list.ItemsSource = VKSongs;



        }
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly MediaPlayer mediaPlayer = new MediaPlayer();
        bool IsPlaying;
        private void PlayingSong(object sender, RoutedEventArgs e)
        {
            Song song = (Song)list.SelectedItem;
            SongTitle.Text = song.Title;
            SongArtist.Text = song.Artist;
            TimeSpan ts = TimeSpan.FromSeconds(song.Time);
            SongTime.Text = ts.ToString(@"mm\:ss");
            TimeLine.Minimum = 0;
            TimeLine.Maximum = song.Time;
            mediaPlayer.Open(new Uri(song.Url));
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;
            mediaPlayer.MediaEnded += SongEnding;
            timer.Start();
            IsPlaying = true;
            buttonPlay.Source = new BitmapImage(new Uri("Resources/pause.png", UriKind.Relative));
            mediaPlayer.Play();
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
            if (list.SelectedIndex < list.Items.Count - 1)
                list.SelectedIndex += 1;
            else
                list.SelectedIndex = 0;
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
            if (list.SelectedIndex > 0)
                list.SelectedIndex -= 1;
            else
                list.SelectedIndex = list.Items.Count - 1;
        }
        private void BTClickNext(object sender, MouseEventArgs e)
        {
            if (list.SelectedIndex < list.Items.Count - 1)
                list.SelectedIndex += 1;
            else
                list.SelectedIndex = 0;
        }
        private void ChangeVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = e.NewValue;
        }
        private void ChangePositionSong(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(e.NewValue);
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
        private void ShwoBoxWithUrl(object sender, RoutedEventArgs e)
        {
            Song song = (Song)list.SelectedItem;
            Clipboard.SetText(song.OriginalUrl);
            MessageBox.Show(song.Url);
        }
    }

}
