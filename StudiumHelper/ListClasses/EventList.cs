using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using StudiumHelper.EventClass;
using System.Runtime.InteropServices;
using StudiumHelper.CustomEventArgs;

namespace StudiumHelper
{

    public delegate void ListSizeChanged(object sender, EventArgs e);
    public delegate void EventAdded(object sender, EventAddedArgs e);
    public class EventList {

        private static String pathDirectoryEventList = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "StudiumHelper");
        private static String pathEventList = Path.Combine(pathDirectoryEventList, "eventList.json");
        private static Boolean isCreated = false;
        private static int length = 0;
        public static event EventHandler ListSizeChanged;
        public static event EventHandler<EventAddedArgs> EventAdded;

        public static List<Event> eventList;
        public EventList()
        {
            if (!isCreated)
            {
                eventList = new List<Event>();
                Note.MemoChangedInNote += OnMemoListChanged;
            }
            isCreated = true;
        }

        public int GetLength()
        {
            return length;
        }
        
        public void Remove(Event thisEvent)
        {
            eventList.Remove(thisEvent);
            ListItemChanged();

        }

        public void SetLength(int input)
        {
            length = input;
        }
        public List<Event> GetList()
        {
            return eventList;
        }

        public bool Find( String name )
        {
            foreach( Event thisEvent in eventList )
            {
                if( name == thisEvent.name )
                    return true;
            }
            return false;
        }
        public void AddEvent(Event e1)
        {
            length++;
            eventList.Add(e1);
            //var e = new ListSizeChangedArgs(eventList.Count);
            var e = new EventAddedArgs(e1);
            OnEventAdded(e);
            SortByDate();
            SaveJson();
            OnListSizeChanged(EventArgs.Empty);
            
        }

        public void SetJson()
        {
            try
            {
                string output = File.ReadAllText( pathEventList );
                if( output.Length != 0 )
                {
                    eventList = JsonConvert.DeserializeObject<List<Event>>( output );
                }
                
                if ( eventList != null )
                {
                    SetLength( eventList.Count) ;
                }
                else
                {
                    SetLength( 0 );
                }
                
            }catch( Exception e )
            {
                
            }
            
        }

        public void ListItemChanged( )
        {
            
            SortByDate( );
            SaveJson( );
            OnListSizeChanged( EventArgs.Empty );
        }

        public void SaveJson()
        {
            try {
                var result = JsonConvert.SerializeObject( eventList );
                System.IO.File.WriteAllText( pathEventList, result );
            }
            catch (Exception e) {

            }
        }

        private void GetEventListJson( )
        {
            try
            {
                var result = File.ReadAllText( pathEventList );
                length = result.Length;
            } catch ( Exception e )
            {

            }
        }


        public Event GetNextMeeting(List<Event> events)
        {
            DateTime today = DateTime.Now;
            Event nextEvent = null;

            foreach (Event thisEvent in events)
            {
                if (nextEvent == null) nextEvent = thisEvent;
                int dayOfWeek = (int)thisEvent.dayOfWeek;

                if (thisEvent.dayOfWeek < nextEvent.dayOfWeek) {
                    dayOfWeek = (int)(thisEvent.dayOfWeek + 7);
                }

                if (nextEvent.dayOfWeek - today.DayOfWeek <
                    dayOfWeek - (int)today.DayOfWeek) {

                    nextEvent = thisEvent;
                } else if (nextEvent.dayOfWeek - today.DayOfWeek ==
                   dayOfWeek - (int)today.DayOfWeek) {

                    TimeSpan spanThisMeeting = today - thisEvent.end;
                    TimeSpan spanNextMeeting = today - nextEvent.end;

                    if (spanThisMeeting > spanNextMeeting) {
                        nextEvent = thisEvent;
                    }
                }
            }

            return nextEvent;

        }
        //Wirf NullPointerException wenn es nur 1 Veranstaltung gibt
        public Event GetEventAfter(Event nextEvent)
        {
            if (length == 0)
            {
                throw new NullReferenceException("Es gibt nicht genug Objekte um zu suchen");
            }

            if (length == 1) {
                
                return nextEvent;
            }

            return eventList[1]; 
        }

        public Event getFirstEvent() {
            if(eventList.Count != 0)
            {
                return eventList[0];
            }else
            {
                return null;
            }
            
        }
        /*
        public DateTime GetNextDate(Event studyEvent)
        {
            return studyEvent.UpdateDate();
        }
        */

        public Boolean IsEventOver(Event studyEvent)
        {
            if (( studyEvent.end - DateTime.Now ).TotalSeconds < 0)
            {
                return true;
            }
            return false;
        }
        
        /*
        public DateTime UpdateEventDateTime(Event ev)
        {
            DateTime now = DateTime.Now;
            DateTime dateOfEvent = new DateTime(now.Year, now.Month, now.Day, ev.hour.Hour, ev.hour.Minute, ev.hour.Second);

            if (ev.hour.DayOfWeek == dateOfEvent.DayOfWeek && (ev.end - now).TotalSeconds < 0)
            {
                //Console.WriteLine(" + 7Tage");
                return dateOfEvent.AddDays(7);
            }

            for (int i = 0; i < 7 || ev.hour.DayOfWeek != dateOfEvent.DayOfWeek; i++)
            {
                dateOfEvent = dateOfEvent.AddDays(1);
                //Console.WriteLine("Tag hinzugefügt");
            }

            return dateOfEvent;
        }

        
        public DateTime GetEndOfEventDateTime(Event ev)
        {
            DateTime now = DateTime.Now;
            DateTime dateOfEvent = new DateTime(now.Year, now.Month, now.Day, ev.end.Hour, ev.end.Minute, ev.end.Second);

            if( ev.hour.DayOfWeek == dateOfEvent.DayOfWeek && ( ev.end - now ).TotalSeconds < 0 )
            {
                Console.WriteLine(" + 7 Tage");
                return dateOfEvent.AddDays(7);
            }

            for ( int i = 0; i < 7 || ev.hour.DayOfWeek != dateOfEvent.DayOfWeek; i++ )
            {
                dateOfEvent.AddDays( 1 );
            }

            return dateOfEvent;
        }
        */
        public String HourOutputWithoutSeconds(DateTime dt)
        {
            string output = dt.TimeOfDay.ToString();

            return output.Substring(0, 5);
        }

        public void SortByDate()
        {
            if(eventList.Count != 0)
            {
                UpdateDates();
                eventList.Sort((x, y) => DateTime.Compare(x.hour, y.hour));
               // Console.WriteLine(getFirstEvent().name + " Anfang" + getFirstEvent().hour);
            }
            
        }
        
        private void UpdateDates()
        {
            DateTime now = DateTime.Now;
            foreach ( Event thisEvent in eventList )
            {
                if (( thisEvent.end - now ).TotalMinutes < 0)
                {
                    thisEvent.UpdateDate();
                }    
            }
        }

        public void OnMemoListChanged(Object sender, EventArgs e)
        {
            Console.WriteLine("kam in OnMemoListChanged");
            OnListSizeChanged( EventArgs.Empty );
        }

        public void OnListSizeChanged(EventArgs e)
        {
            Console.WriteLine("kam in OnListSizeChanged");
            EventHandler handler = ListSizeChanged;

            if (handler != null)
            {
                handler(this, e);
                //Console.WriteLine("Event Invoke");
            }
            
        }

        public void OnEventAdded(EventAddedArgs e)
        {
            Console.WriteLine("kam in OnListSizeChanged");
            EventHandler<EventAddedArgs> handler = EventAdded;

            if (handler != null)
            {
                handler(this, e);
                //Console.WriteLine("Event Invoke");
            }

        }

    }

}
