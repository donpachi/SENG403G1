using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SENG403
{
    class SoundModule
    {
        String paths = Path.Combine(Directory.GetCurrentDirectory(), "\\Sounds");
        
        public void playSound()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            //player.SoundLocation = path;
            System.Diagnostics.Debug.WriteLine("directory:   "+paths);
        }
    }
}
