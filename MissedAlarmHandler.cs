using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SENG403
{
    public class MissedAlarmHandler
    {
        static public event EventHandler MissedAlarm;

        static public void triggerMissedAlarmEvent(object sender, EventArgs args)
        {
            if (MissedAlarm != null) MissedAlarm(sender, args);
        }
    }

    public class MissedAlarmEventArgs : EventArgs
    {

    }
}
