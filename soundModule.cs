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
        SoundPlayer player;
        Boolean playing = false;

        //
        public void playSound()
        {
            //TODO: some way to choose which sound is played
            // - probably make it so that another class can choose the sound (so user can pick which alarm sound
            //   should play with the respective alarm
            Stream theSound = Resources.sawarp1;

            try
            {
                player = new SoundPlayer(theSound);
                player.PlayLooping();
                playing = true;
            }
            catch(FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("Error: sound file not found");
            }
        }

        //
        public void stopSound()
        {
            player.Stop();
            playing = false;
            player.Dispose();
        }

        public Boolean isPlaying()
        {
            return playing;
        }


    }
}
