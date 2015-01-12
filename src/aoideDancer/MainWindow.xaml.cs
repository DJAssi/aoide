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
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static FMOD.System system = null;
        public bool is_playlist = true;

        public MainWindow()
        {
            InitializeComponent();
            uint version = 0;
            FMOD.RESULT result;
            result = FMOD.Factory.System_Create(ref system);
            result = system.getVersion(ref version);
            if (version < FMOD.VERSION.number)
            {
                MessageBox.Show("Error!  You are using an old version of FMOD " + version.ToString("X") + ".  This program requires " + FMOD.VERSION.number.ToString("X") + ".");
                this.Close();
                return;
            }

            result = system.init(2, FMOD.INITFLAGS.NORMAL, (IntPtr)null);
            result = system.setStreamBufferSize(64 * 1024, FMOD.TIMEUNIT.RAWBYTES);
            Database.init();
            playerA.SongFinished += playerA_SongFinished;

            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CheckForUpdateCompleted += CurrentDeployment_CheckForUpdateCompleted;
                System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CheckForUpdateAsync();
            }
        }

        void CurrentDeployment_CheckForUpdateCompleted(object sender, System.Deployment.Application.CheckForUpdateCompletedEventArgs e)
        {
            if (e.UpdateAvailable)
            {
                MenuUpdate.Visibility = System.Windows.Visibility.Visible;
            }
        }

        void playerA_SongFinished(object sender, EventArgs e)
        {
            var f = playlistA.getNextSongAndDel();
            if (f == null) return;
            playerA.LoadSongAndPlay(f);
        }



        private void MenuSongEdit_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.Title = "Song zum editieren öffnen";
            dlg.ShowDialog(this);
            if (dlg.FileName == null || dlg.FileName == "") return;
            System.IO.FileInfo fi = new System.IO.FileInfo(dlg.FileName);
            Win_Songeditor se = new Win_Songeditor(fi);
            se.Show();
        }



        private void MenuSongoptimize_Click(object sender, RoutedEventArgs e)
        {
            Win_Songoptimizer se = new Win_Songoptimizer();
            se.Show();
        }


        private void ctl_songsearch_StartSongClick(object sender, EventArgs e)
        {
            if (!playerA.is_playing())
            {
              
                playerA.LoadSong(((SongEventArgs)e).song.Datei);
            }
            else
            {
                playlistA.add(((SongEventArgs)e).song.Datei);
            }
        }

        private void playlistA_StartSongClick(object sender, EventArgs e)
        {
            playerA.LoadSong(((SongEventArgs)e).song.Datei);
        }

        private void MenuPlaylist_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuSettings_Click(object sender, RoutedEventArgs e)
        {
            (new Win_Settings()).ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            playerA.Dispose();
            Application.Current.Shutdown(1);
        }





    }
}
