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
using System.Windows.Shapes;

namespace aoideDancer
{
    /// <summary>
    /// Interaktionslogik für Win_Songeditor.xaml
    /// </summary>
    public partial class Win_Songeditor : Window
    {

        System.IO.FileInfo _datei = null;
        Song _song = null;
        bool is_loading = true;

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        long sw_lasttap = 0;
        int sw_counter = 0;

        public Win_Songeditor()
        {
            Dictionary<string,string> dict = CommandlineParser.getDictionary();
            if (dict.ContainsKey("edit"))
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(dict["edit"]);
                if (!fi.Exists)
                {
                    MessageBox.Show("Die Musikdatei für den Editor gibt es nicht.");
                    Application.Current.Shutdown();
                    return;
                }
                InitializeComponent();
                
                start(fi);
            }
        }


        public Win_Songeditor(System.IO.FileInfo Musikdatei)
        {
            InitializeComponent();
            start(Musikdatei);
        }

        public void start(System.IO.FileInfo Musikdatei)
        {
            foreach (var a in funcs.dances)
            {
                var b = new ComboBoxItem();
                b.Content = a.Value.Name;
                b.Tag = a.Key;
                FldDance.Items.Add(b);
            }
            _datei = Musikdatei;
            _song = new Song(Musikdatei);
            FldFilename.Content = _datei.FullName;
            FldArtist.Text = _song.Artist;
            FldTitle.Text = _song.Title;
            FldVersion.Text = _song.Version;
            FldDance.Text = funcs.dancename(_song.getDance());
            FldDanceTpM.Text = _song.getDanceTpM().ToString();
            FldFirstStep.Text = _song.FirstDanceBeat.ToString();
            is_loading = false;
        }

        private void MenuSongOpen_Click(object sender, RoutedEventArgs e)
        {
           
            
            
        }

        private void MenuSongSave_Click(object sender, RoutedEventArgs e)
        {
            //_song.Save(_datei.FullName);
        }

        private void FldArtist_changed(object sender, TextChangedEventArgs e)
        {
            if (is_loading) return;
            _song.Artist = FldArtist.Text;
        }

        private void Form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_song.is_playing()) _song.stop();
            if (_song.is_edited)
            {
                MessageBoxResult mbr = MessageBox.Show("Die Daten wurden geändert. Möchten Sie die Daten speichern?", "mp3 Editor", MessageBoxButton.YesNoCancel);
                if (mbr == MessageBoxResult.Cancel) e.Cancel = true;
                if (mbr == MessageBoxResult.Yes) _song.save();
            }
        }

        private void FldDance_change(object sender, SelectionChangedEventArgs e)
        {
            if (is_loading) return;
            _song.setDance(((ComboBoxItem)FldDance.Items[FldDance.SelectedIndex]).Tag.ToString());
        }

        private void FldDanceTpM_changed(object sender, TextChangedEventArgs e)
        {
            if (is_loading) return;
            try
            {
                _song.setDanceTpM(float.Parse(FldDanceTpM.Text));
            }
            catch (Exception ex) { }
        }

        private void BtnTap_click(object sender, RoutedEventArgs e)
        {
            if (!sw.IsRunning || sw_lasttap+5000<sw.ElapsedMilliseconds)
            {
                sw.Reset();
                sw.Start();
                sw_counter = 0;
                sw_lasttap = 0;
            }
            else
            {
                sw_counter++;
                sw_lasttap = sw.ElapsedMilliseconds;
                long em = sw.ElapsedMilliseconds;
                var d = funcs.dances[_song.getDance()];
                float o = 0;
                if (d.QTakt == 3) o = (20000f / (sw.ElapsedMilliseconds / sw_counter)); else o = (60000f / (sw.ElapsedMilliseconds / sw_counter));
                if (o < d.TpMmin) o = o * 2f;
                if (o > d.TpMmax) o = o / 2f;
                if (o > d.TpMmax) o = o / 2f;
                FldDanceTpM.Text = o.ToString();
            }
        }

        private void BtnSongPlay_click(object sender, RoutedEventArgs e)
        {
            if (!_song.is_playing())
            {
                _song.play();
            }
            else
            {
                _song.stop();
            }
        }

        private void FldTitle_changed(object sender, TextChangedEventArgs e)
        {
            if (is_loading) return;
            _song.Title = FldTitle.Text;
        }

        private void FldVersion_changed(object sender, TextChangedEventArgs e)
        {
            if (is_loading) return;
            _song.Version = FldVersion.Text;
        }

        private void BtnSave_click(object sender, RoutedEventArgs e)
        {
            _song.save();
        }

        private void FldFirstStep_changed(object sender, TextChangedEventArgs e)
        {
            if (is_loading) return;
            try
            {
                _song.FirstDanceBeat = float.Parse(FldFirstStep.Text);
            }
            catch (Exception ex) { }
        }

        private void BtnTapFirstStep_click(object sender, RoutedEventArgs e)
        {
            if (!_song.is_playing()) { _song.play(); return; }
            FldFirstStep.Text = _song.getPosition().ToString();
            uint d = _song.DurationMS;
            if (d > 0) _song.DurationMS = d;
        }

        private void BtnOptiFilename_Click(object sender, RoutedEventArgs e)
        {
            String a = _song.Artist +" - "+_song.Title;
            if (_song.Version != null) a+= " ("+_song.Version+")";
            if (_song.Year != null) a+= " ["+_song.Year+"]";
            if (_song.getDance() != null) a+= " ["+_song.getDance()+Math.Round(_song.getDanceTpM(),0).ToString()+"]";
            a += ".mp3";
            FldFilename.Content = a;
        }
    }
}
