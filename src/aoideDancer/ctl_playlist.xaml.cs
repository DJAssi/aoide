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
using System.Collections.ObjectModel;

namespace aoideDancer
{
    /// <summary>
    /// Interaktionslogik für ctl_playlist.xaml
    /// </summary>
    public partial class ctl_playlist : UserControl
    {
        public event EventHandler StartSongClick;
        ObservableCollection<PlaylistSong> _empList = new ObservableCollection<PlaylistSong>();
        
        public ctl_playlist()
        {
            InitializeComponent();
            /*_empList.Add(new PlaylistSong(new System.IO.FileInfo("c:\\test.mp3"), "Hallo","CC"));
            _empList.Add(new PlaylistSong(new System.IO.FileInfo("c:\\test.mp3"), "Hallo2","DF"));
            _empList.Add(new PlaylistSong(new System.IO.FileInfo("c:\\test.mp3"), "Hallo3","LW"));*/
            listbox1.ItemsSource = _empList;

            Style itemContainerStyle = new Style(typeof(ListBoxItem));
            itemContainerStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));
            itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(s_PreviewMouseLeftButtonDown)));
            itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.DropEvent, new DragEventHandler(listbox1_Drop)));
            listbox1.ItemContainerStyle = itemContainerStyle;

        }

        public void add(System.IO.FileInfo datei) {
            Song s = new Song(datei);
            _empList.Add(new PlaylistSong(datei, s.Artist+" - "+s.Title, s.getDance()));
        }

        public bool getAutoPlay()
        {
            return (CBAutoPlay.IsChecked == true);
        }

        public System.IO.FileInfo getNextSongAndDel()
        {
            if (_empList.Count == 0) return null;
            System.IO.FileInfo o = _empList[0].datei;
            _empList.RemoveAt(0);
            return o;
        }

        void s_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (sender is ListBoxItem)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                draggedItem.IsSelected = true;
            }
        }

        void listbox1_Drop(object sender, DragEventArgs e)
        {
            PlaylistSong droppedData = e.Data.GetData(typeof(PlaylistSong)) as PlaylistSong;
            PlaylistSong target = ((ListBoxItem)(sender)).DataContext as PlaylistSong;

            int removedIdx = listbox1.Items.IndexOf(droppedData);
            int targetIdx = listbox1.Items.IndexOf(target);

            if (removedIdx < targetIdx)
            {
                _empList.Insert(targetIdx + 1, droppedData);
                _empList.RemoveAt(removedIdx);
            }
            else
            {
                int remIdx = removedIdx + 1;
                if (_empList.Count + 1 > remIdx)
                {
                    _empList.Insert(targetIdx, droppedData);
                    _empList.RemoveAt(remIdx);
                }
            }
        }

        private void listbox1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_empList.Count == 0) return;
            PlaylistSong b = _empList[listbox1.SelectedIndex];
            var ea = new SongEventArgs();
            ea.song = new Song(b.datei);
            if (StartSongClick != null) StartSongClick(this, ea);
        }

    }

    public class PlaylistSong
    {
        public string Name { get; set; }
        public string DanceName { get; set; }
        public System.IO.FileInfo datei { get; set; }

        public PlaylistSong(System.IO.FileInfo datei2, String name2, String dancecode)
        {
            this.datei = datei2;
            this.Name = name2;
            this.DanceName = funcs.dancename(dancecode);
        }
    }
}
