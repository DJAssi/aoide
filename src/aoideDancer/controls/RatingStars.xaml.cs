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
    /// Interaktionslogik für RatingStars.xaml
    /// </summary>
    public partial class RatingStars : UserControl
    {
        public RatingStars()
        {
            InitializeComponent();
        }

        private void BGGrid_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos= e.GetPosition(this);
            StarsOn.Margin = new Thickness(pos.X / BGGrid.Width,0,0,0);
        }

        private void BGGrid_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void BGGrid_MouseLeave(object sender, MouseEventArgs e)
        {

        }
    }
}
