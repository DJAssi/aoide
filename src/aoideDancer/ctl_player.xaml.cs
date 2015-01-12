using System;
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

namespace aoideDancer
{
    /// <summary>
    /// Interaktionslogik für ctl_player.xaml
    /// </summary>
    public partial class ctl_player : UserControl
    {
        private Song song = null;
        private Song song2 = null;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private bool is_answered = false;
        public bool is_finished = false;
        public event EventHandler SongFinished;

        public ctl_player()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            if (song != null) song.dispose();
            if (song2 != null) song2.dispose();
        }

        public void LoadSong(System.IO.FileInfo datei)
        {
            if (song != null) song.dispose();
            song = new Song(datei);
            LabelArtist.Content = song.Artist;
            LabelTitle.Content = song.Title;
            var imgcover = song.getCover();
            if (imgcover != null) CDCover.Source = imgcover;
            if (song.getDance() == "") LabelDance.Content = "unbekannter Tanz";
            else if (song.getDanceTpM() > 0) LabelDance.Content = funcs.dancename(song.getDance()) + " mit " + song.getDanceTpM().ToString("N1") + " Takte/min.";
            else LabelDance.Content = funcs.dancename(song.getDance());
            LabelZeit.Content = funcs.format_time(0) + "/" + funcs.format_time(song.Duration);
            SliderSpeed.Value = 1;
            drawPitch();
            //CDCover.Source = 
            //song.getCoverImage();
            //song.getCoverImage();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0,0,250);
            dispatcherTimer.Start();
            is_answered = false;
            is_finished = false;
            song.Finished += song_Finished;
        }

        public void LoadSongAndPlay(System.IO.FileInfo datei)
        {
            LoadSong(datei);
            song.play();
        }

        void song_Finished(object sender, EventArgs e)
        {
            Dispatcher.Invoke((Action) delegate() {
                if (SongFinished != null) SongFinished(this, null);
            });
        }

        private void OnBtnPlayClick(object sender, RoutedEventArgs e)
        {
            if (song == null) return;
            if (song.is_playing())
            {
                song.pause();
            }
            else { song.play();
            drawPitch();
            }
            
        }

        private void OnBtnSpeedUpClick(object sender, RoutedEventArgs e)
        {
            song.setSpeedUp(1);
            drawPitch();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            float p = song.getPosition();
            float d = song.Duration;
            SongProgress.Maximum = Math.Max(1,d*10);
            SongProgress.Value = p*10;
            LabelZeit.Content = funcs.format_time(p) + "/" + funcs.format_time(d);
            LabelRestzeit.Content = funcs.format_time(d - p);
            ProgressVolume.Value = song.getVolume() * 100f;
            if (!is_answered && p > 5 )
            {
                if (BtnTanzansage.IsChecked == true)
                {

                    System.IO.FileInfo[] files = new System.IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\aoide_voices").GetFiles(song.getDance() + "*.mp3");
                    if (files.Length > 0)
                    {
                        song.setVolume(25,0.5f);
                        Random rnd = new Random();
                        int a = (int)Math.Round(rnd.NextDouble() * (files.Length - 1), 0);
                        song2 = new Song(files[a]);
                        
                            song2.Finished += song2_Finished;
                        song2.play();
                    }
                }
                is_answered = true;

            }
        }

        private void song2_Finished(object sender, EventArgs e)
        {
            song2.dispose();
            song.setVolume(100,2);
        }

        private void OnPositionClick(object sender, MouseButtonEventArgs e)
        {
            ProgressBar pb = (ProgressBar)sender;
            Point p = e.GetPosition((IInputElement)sender);
            song.setPosition((float)(song.Duration*p.X/pb.Width));
        }

        private void OnBtnStopClick(object sender, RoutedEventArgs e)
        {
            if (song == null) return;
            song.stop();
        }

        private void OnFileDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            LoadSong(new System.IO.FileInfo(files[0]));
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            bool isCorrect = true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                if (files.Length != 1) isCorrect = false;
                System.IO.FileInfo info = new System.IO.FileInfo(files[0]);
                if (info.Extension != ".mp3") isCorrect = false;
            }

            if (isCorrect) e.Effects = DragDropEffects.All; else e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void OnBtnSpeedDownClick(object sender, RoutedEventArgs e)
        {
            song.setSpeedDown(1);
            drawPitch();
        }

        private void drawPitch()
        {
            float a1 = song.getCurrentSpeed();
            if (song.getDanceTpM() > 0)
            {
                if (a1 == -1) LabelCurrentTpM.Content = (song.getDanceTpM()).ToString("N1") + " TpM";
                else LabelCurrentTpM.Content = (song.getDanceTpM() * a1).ToString("N1") + " TpM";
            }
            else
            {
                if (a1 == -1) LabelCurrentTpM.Content = "100,0%";
                else LabelCurrentTpM.Content = (song.getCurrentSpeed() * 100).ToString("N1") + "%";
            }
            if (a1 != -1) SliderSpeed.Value = song.getCurrentSpeed();
        }

        private void OnSliderSpeedChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (song == null) return;
            Slider ele = (Slider)sender;
            song.setSpeedPercent((float)ele.Value *100f);
            drawPitch();
        }

        private void OnSliderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (song == null) return;
            if (e.RightButton == MouseButtonState.Pressed) song.setSpeedPercent(100f);
            drawPitch();
        }

        private void BtnTalkOverDown(object sender, MouseButtonEventArgs e)
        {
            if (song == null) return;
            song.setVolume(25);
            BtnTalkOver.Content = "U";
        }

        private void BtnTalkOverUp(object sender, MouseButtonEventArgs e)
        {
            if (song == null) return;
            song.setVolume(100);
            BtnTalkOver.Content = "V";
        }

        public bool is_nosong()
        {
            return (song == null || is_finished);
        }

        public bool is_playing()
        {
            if (song == null) return false;
            return song.is_playing();
        }
    }
}
