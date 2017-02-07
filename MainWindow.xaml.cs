using System;
using System.Collections.Generic;
using System.IO;
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

namespace SENG403
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SoundModule sound = new SoundModule();
        

        public MainWindow()
        {
            InitializeComponent();
        }

        // TEMPORARY - Austin
        /*
        // This button event simulates what would be done in the UI to select a sound the user wants
        // for the particular alarm the user is setting up.
        private void soundbutton_Click(object sender, RoutedEventArgs e)
        {
            if (sound.isPlaying())
                sound.stopSound();
            
            else{
                try{
                    sound.playSound(sound.getSound(0));
                }
                catch(Exception)
                {
                    System.Diagnostics.Debug.WriteLine("Error: no .wav files in Sounds folder");
                }
            }
        }*/

    }
}
