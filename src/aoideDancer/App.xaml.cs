using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace aoideDancer
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Dictionary<string, string> dict = CommandlineParser.getDictionary();
            if (dict.ContainsKey("edit")) StartupUri = new Uri("Win_Songeditor.xaml", UriKind.Relative);
        }


    }
}
