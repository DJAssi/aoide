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
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class ctl_songsearch : UserControl
    {

        public event EventHandler StartSongClick;
        public System.Data.DataTable dt;

        System.Threading.Thread thread01 = null;


        public ctl_songsearch()
        {
            InitializeComponent();

            foreach (var a in funcs.dances)
            {
                var b = new ComboBoxItem();
                b.Content = a.Value.Name;
                b.Tag = a.Key;
                SelectDance.Items.Add(b);
            }

            for (int i = DateTime.Now.Year; i > 1800; i--)
            {
                var b = new ComboBoxItem();
                b.Content=i;
                b.Tag=i;
                SelectYear.Items.Add(i);
                if (i % 10 == 0) {
                    b = new ComboBoxItem();
                    b.Content = i.ToString()+"er";
                    b.Tag = i.ToString().Substring(0,3);
                    SelectYear.Items.Add(b);
                }
            }




                dt = new System.Data.DataTable("songs");
            dt.Columns.Add("Artist");
            dt.Columns.Add("Titel");
            dt.Columns.Add("Tanzcode");
            dt.Columns.Add("Tanz");
            dt.Columns.Add("TpM");
            dt.Columns.Add("Duration");
            dt.Columns.Add("Year");
            dt.Columns.Add("Cover");
            dt.Columns.Add("file");

            
            thread01 = new System.Threading.Thread(delegate() {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

                System.IO.FileInfo[] files = new System.IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)).GetFiles("*.mp3",System.IO.SearchOption.AllDirectories);
                Dispatcher.Invoke(delegate() { 
            ProgressIndexer.Maximum = files.Length;
                });
            sw.Start();
            long i = 0;
            foreach (System.IO.FileInfo file in files)
            {
                Song s = new Song(file);
                if (s.getDance() == "") { s.dispose(); continue; }
                System.Data.DataRow row = dt.NewRow();
                row["Artist"] = s.Artist;
                row["Titel"] = s.Title;
                row["Tanzcode"] = s.getDance();
                row["Tanz"] = s.Dancename;
                row["TpM"] = s.getDanceTpM();
                row["Duration"] = funcs.format_time(s.Duration);
                row["Year"] = s.Year;
                row["Cover"] = s.getCover();
                row["file"] = file.FullName;
                dt.Rows.Add(row);
                s.dispose();
                i++;
                if (this == null) return;
                if (sw.ElapsedMilliseconds > 5000)
                {
                    if (this == null) return;
                    Dispatcher.Invoke(delegate()
                    {
                        if (ProgressIndexer == null) return;
                        ProgressIndexer.Value = i;
                        ProgressIndexer.ToolTip = i + "/" + ProgressIndexer.Maximum;
                    });
                    sw.Restart();
                }
            }
            Dispatcher.Invoke(delegate()
            {
                ProgressIndexer.Value = i;
                ProgressIndexer.Visibility = System.Windows.Visibility.Collapsed;
            });
            });
            thread01.IsBackground = true;
            thread01.Start();




            gridSongs.DataContext = dt;
            //dt.DataSet.WriteXml(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\aoide_songs.xml");

        }

        public void Dispose()
        {
            if (thread01 != null) thread01.Abort();
        }

        public List<System.IO.FileInfo> scanDir(System.IO.DirectoryInfo dir)
        {
            List<System.IO.FileInfo> o = new List<System.IO.FileInfo>();
            foreach (System.IO.FileInfo file in dir.GetFiles("*.mp3"))
            {
                o.Add(file);
            }
            foreach (System.IO.DirectoryInfo di in dir.GetDirectories()) o.AddRange(scanDir(di));
            return o;
        }

        private void searchgrid_doubleclick(object sender, MouseButtonEventArgs e)
        {
            var a = gridSongs.SelectedIndex;
            if (a == -1) return;
            System.Data.DataRowView b = (System.Data.DataRowView)gridSongs.Items[a];
            String c = (String)b.Row["file"];
            //player.LoadSong(new System.IO.FileInfo(c));
            var ea = new SongEventArgs();
            ea.song = new Song(new System.IO.FileInfo(c));
            if (StartSongClick != null) StartSongClick(this,ea);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboBoxItem ele = (ComboBoxItem)SelectDance.Items[SelectDance.SelectedIndex];
                String tanz = (String)ele.Tag;
                var dt2 = dt.Clone();
                System.Data.DataRow[] liste = dt.Select("Tanzcode = '" + tanz + "'");

                foreach (var row in liste)
                {
                    System.Data.DataRow raw = dt2.NewRow();
                    raw["Artist"] = row["Artist"];
                    raw["Titel"] = row["Titel"];
                    raw["Tanzcode"] = row["Tanzcode"];
                    raw["Tanz"] = row["Tanz"];
                    raw["TpM"] = row["TpM"];
                    raw["Duration"] = row["Duration"];
                    raw["Year"] = row["Year"];
                    raw["Cover"] = row["Cover"];
                    raw["file"] = row["file"];
                    dt2.Rows.Add(raw);
                }

                gridSongs.DataContext = dt2;
            } catch (Exception ex) {
                System.Threading.Thread.Sleep(500);
                Button_Click(sender,e);
            }

        }

        private void SelectDance_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Click(this, null);
        }

        private void gridSongs_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.E) return;
            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
            {
                var a = gridSongs.SelectedIndex;
                System.Data.DataRowView b = (System.Data.DataRowView)gridSongs.Items[a];
                String c = (String)b.Row["file"];
                new Win_Songeditor(new System.IO.FileInfo(c)).Show();
            }
        }
    }

    public partial class SongEventArgs : RoutedEventArgs
    {
        public Song song;
    }
}
