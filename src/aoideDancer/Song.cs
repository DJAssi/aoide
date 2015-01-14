using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace aoideDancer
{
    public class Song {

        private System.IO.FileInfo _datei = null;
        private float _original_frequency;
        private FMOD.Sound sound = null;
        private FMOD.Channel channel = null;
        public IdSharp.Tagging.ID3v2.ID3v2Tag id3v2 = null;
        private float _speed = 100f;
        private bool is_disposed = false;
        public bool is_edited = false;
        

        public event EventHandler Finished;

        public Song(System.IO.FileInfo datei) {
            if (!datei.Exists) return;
            _datei = datei;
            id3v2 = new IdSharp.Tagging.ID3v2.ID3v2Tag(_datei.FullName);
            id3v2.PropertyChanged += id3v2_PropertyChanged;
            var b = 1 + 1;
        }

        void id3v2_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.is_edited = true;
        }

        public void dispose()
        {
            if (sound != null) sound.release();
            //if (channel != null) channel.
        }

        public System.IO.FileInfo Datei {
            get { return _datei; }
        }

        public void save()
        {
            if (sound != null) sound.release();
            if (_datei.IsReadOnly) throw new Exception("Datei ist nicht beschreibbar. Read-Only");
            id3v2.Save(_datei.FullName);
            is_edited = false;
        }

        public void play()
        {
            FMOD.RESULT result;
            FMOD.OPENSTATE openstate = 0;
            uint percentbuffered = 0;
            bool starving = false;
            bool busy = false;

            result = MainWindow.system.createSound(_datei.FullName, (FMOD.MODE.HARDWARE | FMOD.MODE._2D | FMOD.MODE.CREATESTREAM | FMOD.MODE.NONBLOCKING), ref sound);
            
            while (true)
            {
                result = sound.getOpenState(ref openstate, ref percentbuffered, ref starving, ref busy);
                if (openstate == FMOD.OPENSTATE.READY) break;
                if (openstate == FMOD.OPENSTATE.ERROR) return;
            }
            System.Threading.Thread.Sleep(500);
            result = MainWindow.system.playSound(FMOD.CHANNELINDEX.FREE, sound, false, ref channel);
            if (channel == null) { return; }
            channel.getFrequency(ref _original_frequency);
            //this.SpeedPercent = _speed*100f;
            System.Threading.Thread fthread = new System.Threading.Thread(delegate() {
                if (this.is_disposed) return;
                uint a1 = 0;
                uint a2 = 0;
                while (true)
                {
                    try {
                         a1 = this.DurationMS;
                    this.channel.getPosition(ref a2, FMOD.TIMEUNIT.MS);
                    }
                    catch (Exception ex) { a1 = 0; a2 = 0; }
                    if (a1 != 0 && a1 - 250 < a2)
                    {
                        App.Current.Dispatcher.Invoke(delegate()
                        {
                            if (Finished != null) Finished(this, null);
                        });
                        
                    
                        return;
                    
                        }
                    
                    System.Threading.Thread.Sleep(100);
                }
            });
            fthread.IsBackground = true;
            fthread.Start();
        }



        public bool is_playing()
        {
            if (channel == null) return false;
            bool a = false;
            channel.isPlaying(ref a);
            return a;
        }
        
        public bool is_paused()
        {
            if (channel == null) return false;
            bool a = false;
            channel.getPaused(ref a);
            return a;
        }

        public void pause()
        {
            channel.setPaused(!is_paused());
        }
        public void pause(bool forcevalue)
        {
            channel.setPaused(forcevalue);
        }
        public void stop()
        {
            if (channel == null) return;
            channel.stop();
        }

        public float Duration
        {
            get {
                return (float)this.DurationMS / 1000f;
            }
        }

        public uint DurationMS
        {
            get {
                var a2 = id3v2.LengthMilliseconds;
                if (a2 != null) return (uint)a2;
                if (sound == null) return 0;
                uint a1 = 0;
                this.sound.getLength(ref a1, FMOD.TIMEUNIT.MS);
                return a1;
            }
            set {
                if (id3v2.LengthMilliseconds == (int)value) return;
                id3v2.LengthMilliseconds = (int)value;
            }
        }

        public String Genre
        {
            get { return id3v2.Genre; }
        }

        public String ISRC
        {
            get { return id3v2.ISRC; }
        }

        public float SpeedPercent
        {
            get { return _speed; }
            set { _speed = value;
            if (channel != null)
            {
                float a = 0;
                channel.getFrequency(ref a);
                channel.setFrequency(a * 0.01f * _speed);
            }
            
            }

        }




        

        public void setSpeedUp(decimal value)
        {
            if (channel == null) return;
            float a = 0;
            channel.getFrequency(ref a);
            channel.setFrequency(a*1.01f);
        }
        public void setSpeedDown(decimal value)
        {
            float a = 0;
            if (channel == null) return;
            channel.getFrequency(ref a);
            channel.setFrequency(a * 0.99f);
        }

        public void setSpeedPercent(float value)
        {
            if (channel == null) return;
            channel.setFrequency(_original_frequency * value/100f);
        }

        public float getPosition()
        {
            uint pos = 0;
            if (channel == null) return 0;
            channel.getPosition(ref pos,FMOD.TIMEUNIT.MS);
            return (float)pos/1000;
        }
        public void setPosition(float sekunden)
        {
            if (channel == null) return;
            channel.setPosition((uint) sekunden*1000, FMOD.TIMEUNIT.MS);
        }

        public float getCurrentSpeed()
        {
            if (channel == null) return -1;
            float a = 0;
            channel.getFrequency(ref a);
            return a/_original_frequency;
        }

        public string getDance()
        {
            System.ComponentModel.BindingList<IdSharp.Tagging.ID3v2.Frames.IComments> cl = id3v2.CommentsList;
            foreach (IdSharp.Tagging.ID3v2.Frames.IComments row in cl) {
                if (row.Description == "DANCE") return row.Value;
            }
            return "";   
        }

        public String Dancename
        {
            get { return funcs.dancename(getDance()); }
        }

        public String Year
        {
            get { return id3v2.Year; }
        }

        public String Yearname
        {
            get {
                if (id3v2.Year.Length == 4 && id3v2.Year.Substring(2, 2) == "--") return (int.Parse(id3v2.Year.Substring(0, 2)) + 1) + ". Jh.";
                if (id3v2.Year.Length == 4 && id3v2.Year.Substring(3, 1) == "-") return id3v2.Year.Substring(0, 3) + "0er";
                return id3v2.Year;
            }
        }

        public float getDanceTpM()
        {
            foreach (IdSharp.Tagging.ID3v2.Frames.IComments row in id3v2.CommentsList)
            {
                if (row.Description == "DANCETPM")
                {
                    float v = float.Parse(row.Value.Replace(".",","));
                    if (v > 1000) return v/100f;
                    return v;
                }
            }
            return -1;
        }

        /*public BitmapImage getCoverImage()
        {
            foreach (IdSharp.Tagging.ID3v2.Frames.IAttachedPicture row in id3v2.PictureList)
            {
                //return row.Picture;
            }
            //return null;
        }*/

        public void setVolume(float valuepercent, float timetofade = 0)
        {
            if (channel == null) return;
            if (timetofade == 0)
            {
                channel.setVolume(valuepercent / 100f);
            }
            else
            {
                float a1 = 1f;
                float a2 = valuepercent / 100f;
                int steps = (int)Math.Floor(timetofade / 0.05f);
                channel.getVolume(ref a1);
                new System.Threading.Thread(delegate()
                {
                    for (int i = 0; i < steps; i++)
                    {
                        float v = ((a2 - a1) * ((float)i / (float)steps)) + a1;
                        channel.setVolume(v);
                        System.Threading.Thread.Sleep(50);
                    }
                    channel.setVolume(a2);
                }).Start();

            }

        }

        public float getVolume()
        {
            if (channel == null) return 1;
            float v = -1;
            channel.getVolume(ref v);
            return v;
        }

        public void setDance(string dancecode)
        {
            setComment("DANCE", dancecode);
            id3v2.Publisher = this.getDance() + Math.Round(this.getDanceTpM(), 0).ToString();
        }

        public void setDanceTpM(float value)
        {
            setComment("DANCETPM", value.ToString());
            id3v2.Publisher = this.getDance() + Math.Round(this.getDanceTpM(), 0).ToString();
        }

        public float FirstDanceBeat
        {
            get { String a1 = getComment("DANCEFirstDanceBeat");
            if (a1 == null) return 0; else return float.Parse(a1.Replace(".",","));
            }
            set {
                setComment("DANCEFirstDanceBeat", value.ToString());
            }
        }

        public String getComment(String key)
        {
            System.ComponentModel.BindingList<IdSharp.Tagging.ID3v2.Frames.IComments> cl = id3v2.CommentsList;
            foreach (IdSharp.Tagging.ID3v2.Frames.IComments row in cl) {
                if (row.Description == key) return row.Value;
            }
            return null;   
        }

        public void setComment(string key, String value)
        {
            if (getComment(key) == value) return;
            is_edited = true;
            System.ComponentModel.BindingList<IdSharp.Tagging.ID3v2.Frames.IComments> cl = id3v2.CommentsList;
            foreach (IdSharp.Tagging.ID3v2.Frames.IComments row in cl)
            {
                if (row.Description == key) { row.Value = value; return; }
            }
            IdSharp.Tagging.ID3v2.Frames.IComments b = id3v2.CommentsList.AddNew();
            b.Description = key;
            b.Value = value;
            b.TextEncoding = IdSharp.Tagging.ID3v2.EncodingType.UTF8;
        }


        public Image Image {
            get {
                if (id3v2.PictureList.Count == 0) return null;
                return id3v2.PictureList[0].Picture;
            }
        }

        public String Artist
        {
            get {
                String a = id3v2.Artist; 
                if (a != null && a.Length>0) return a;
                System.Text.RegularExpressions.Regex r1 = new System.Text.RegularExpressions.Regex("(.+) - (.+)\\.mp3");
                var m1 = r1.Match(_datei.Name);
                if (m1.Success) return m1.Groups[1].Value;
                return null;
            }
            set {
                if (id3v2.Artist == value) return;
                id3v2.Artist = value;
            }
        }

        public String Version
        {
            get
            {
                String a = id3v2.Subtitle;
                if (a != null && a.Length > 0) return a;
                System.Text.RegularExpressions.Regex r1 = new System.Text.RegularExpressions.Regex("(.+) - (.+) \\((.+)\\)\\.mp3");
                var m1 = r1.Match(_datei.Name);
                if (m1.Success) return m1.Groups[3].Value;
                return null;
            }
            set { }
        }

        public String Title
        {
            get
            {
                String a = id3v2.Title;
                if (a != null && a.Length > 0) return a;
                System.Text.RegularExpressions.Regex r1 = new System.Text.RegularExpressions.Regex("(.+) - (.+)\\.mp3");
                var m1 = r1.Match(_datei.Name);
                if (m1.Success) return m1.Groups[2].Value;
                return null;
            }
            set
            {
                if (id3v2.Title == value) return;
                id3v2.Artist = value;
            }
        }

        public System.Windows.Media.Imaging.BitmapImage getCover()
        {
            if (id3v2.PictureList.Count == 0) return null;
            byte[] pictureData = id3v2.PictureList[0].PictureData;
            if (pictureData != null)
            {
                try
                {
                    MemoryStream memoryStream = new MemoryStream(pictureData);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();

                    return bitmapImage;
                }
                catch (NotSupportedException)
                {
                }
            }
            return null;
        }
    

        
        









    }
}
