using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudiumHelper
{ 
    public class Event {
        public string name { get; set; }
        public string prof { get; set; }
        public string room { get; set; }
        
        public DateTime hour { get; set; }
        public DateTime end { get; set; }
        public DayOfWeek dayOfWeek { get; set; }

        public String dayOfWeekGerman { get; set; }
       public Event( string name, string prof, string room, DayOfWeek dayOfWeek, DateTime hour, DateTime end )
        {
            this.name = name;
            this.prof = prof;
            this.room = room;
            this.dayOfWeek = dayOfWeek;
            this.hour = hour;
            this.end = end;
        }

        public void UpdateDate()
        {

            DateTime now = DateTime.Now;
            DateTime dateOfEvent = new DateTime(now.Year, now.Month, now.Day, this.hour.Hour, this.hour.Minute, 0);

            for (int i = 0; i <= 7 || this.hour.DayOfWeek != dateOfEvent.DayOfWeek; i++)
            {
                dateOfEvent = dateOfEvent.AddDays(1);
            }

            this.hour = dateOfEvent;

            dateOfEvent = new DateTime(dateOfEvent.Year, dateOfEvent.Month, dateOfEvent.Day, this.end.Hour, this.end.Minute, 0);
            this.end = dateOfEvent;
        }

       
    }
}
