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
    /// Interaktionslogik für Win_Songoptimizer.xaml
    /// </summary>
    public partial class Win_Songoptimizer : Window
    {

        public List<Song> _list = new List<Song>();



        public Win_Songoptimizer()
        {
            InitializeComponent();
            //_list.Add(new Song(new System.IO.FileInfo("c:\\Users\\Andreas\\music\\80s - heaven is a place on earth.mp3")));
            liste1.ItemsSource = _list;
            scanDirectory(new System.IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)));
        }

        public void scanDirectory(System.IO.DirectoryInfo dir)
        {
            foreach (var file in dir.GetFiles("*.mp3", System.IO.SearchOption.AllDirectories))
            {
                Song s = new Song(file);
                if (s.getDance() == "") _list.Add(s);
            }

        }

        private void liste_doubleclick(object sender, MouseButtonEventArgs e)
        {
            Song s = liste1.SelectedItem as Song;
            new Win_Songeditor(s.Datei).Show();
        }

    }






}
