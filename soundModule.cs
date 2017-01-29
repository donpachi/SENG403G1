using SENG403.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace SENG403
{
    class SoundModule
    {

        public void playSound()
        {
            Stream theSound = Resources.squarearp1;

            try
            {
                SoundPlayer player = new SoundPlayer(theSound);
                player.Play();
            }
            catch(FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("Error: sound file not found");
            }
            
           

        }
    }
}
