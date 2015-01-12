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

namespace aoideDancer.controls
{
    /// <summary>
    /// Interaktionslogik für ctl_partyplayer.xaml
    /// </summary>
    public partial class ctl_partyplayer : UserControl
    {

        public List<Song> _NextSongs = new List<Song>();
        private String LastDance = "";
        private int LastDanceCount = 0;
        protected System.Threading.Thread thread01 = null;



        public ctl_partyplayer()
        {
            InitializeComponent();
            Database.songs.Add(new Song(new System.IO.FileInfo("C:\\Users\\Andreas\\Music\\80s - heaven is a place on earth.mp3")));
            ListNextSongs.DataContext = _NextSongs;
        }

        public Song getNextSongAndDel()
        {
            if (_NextSongs.Count == 0) return null;
            Song o = _NextSongs[0];
            _NextSongs.RemoveAt(0);
            return o;
        }

        public void recalc()
        {
            if (thread01 != null && thread01.IsAlive) return;
            thread01 = new System.Threading.Thread(delegate() { 
                foreach (DataGridRow row in quisquilia.Items) {
                   
                
                
                }

                


            });


        }

        public String getForgottenDance(Dictionary<String,PPStatType> Songlist) {
            KeyValuePair<String, PPStatType> _o = new KeyValuePair<string,PPStatType>(null,null);
            float d = 9999f;
            int sumplays = 0;
            foreach(KeyValuePair<String,PPStatType> s in Songlist) {
                sumplays += s.Value.counter;
            }
            foreach(KeyValuePair<String,PPStatType> s in Songlist) {
                float a = (s.Value.counter / sumplays) / (s.Value.targetpercent / 100f);
                if (a < d) { d = a; _o = s;}
            }
            return _o.Key;
        }

        private void BtnRecalc_Click(object sender, RoutedEventArgs e)
        {
            recalc();
        }
    }

    public class PPStatType {
        public String Dancecode = "";
        public int counter = 0;
        public float targetpercent = 0f;
    }

    


}
