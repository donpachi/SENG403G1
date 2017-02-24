using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SENG403
{
    [Serializable]
    class AlarmSettings
    {
        public List<Alarm> alarms { get; set; }

        public AlarmSettings()
        {
            this.alarms = new List<Alarm>();
        }

        public AlarmSettings(List<Alarm> alarmList)
        {
            this.alarms = alarmList;
        }
    }
}
