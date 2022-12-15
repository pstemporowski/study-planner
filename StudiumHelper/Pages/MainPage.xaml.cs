using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace StudiumHelper
{

    /// <summary>
    /// Interaktionslogik für mainPage.xaml
    /// </summary
    /// 
    public partial class MainPage : Page
    {
        private static Event nextEvent = null;
        System.Threading.Timer threadTimer;
        private EventList eventList = new EventList();
        private NotesList notesList = new NotesList();
        public static event EventHandler EventAddBtnClick;
        public MainPage()
        {

            InitializeComponent();

            EventList.ListSizeChanged += mainPageInitialized;
            EventList.ListSizeChanged += UpdateMemos;
            NotesList.MemoChanged += UpdateMemos;
            NotesList.MemoDeleted += UpdateMemos;
            UpdateMemos(null, EventArgs.Empty);

            CheckIfFirstEvent();
        }
        private void UpdateMemos(object sender, EventArgs e)
        {
            CheckIfFirstEvent();
            ToDoNext.ItemsSource = null;
            if(!( nextEvent is null ))
            {
                List< String > memos = new List< String >( );
                Note note = notesList.FindNoteByTitle( nextEvent.name );

                if(note != null)
                {
                    memos = note.GetMemoList();
                }else
                {
                    memos.Add("Keine Aufgaben");
                    ToDoNext.ItemsSource = memos;
                }
                

                if (!(memos is null))
                {
                    ToDoNext.ItemsSource = memos;
                }
            }
        } 

        private void CheckIfFirstEvent()
        {
            int count = eventList.GetLength();

            if (count > 0)
            {
                this.HintGrid.Visibility = Visibility.Hidden;
            }else
            {
                //Console.WriteLine("Ich versuche es!");
                this.HintGrid.Visibility = Visibility.Visible;
            }
            UpdateLayout();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            SetTimerLabel(GetTimeToEvent(nextEvent));
        }

        private void UpdatePage(object sender, EventArgs e)
        {
            Console.WriteLine("Main Page");
        }
        private void mainPageInitialized(object sender, EventArgs e)
        {
            eventList.SetJson( );

            if ( eventList.GetLength() != 0 ) 
            {
                
                nextEvent = eventList.getFirstEvent();

                NotesList.nextEvent = nextEvent;

                SetMainContent( nextEvent );
                Event eventAfter = eventList.GetEventAfter(nextEvent);

                if (eventList.GetList().Count != 0)
                {

                    Note note = notesList.FindNoteByTitle(nextEvent.name);

                    if (note != null)
                    {
                        ToDoNext.ItemsSource = note.memoList;
                    }

                    List<String> allEventsToday = GetAllEventNamesToday();

                    if (allEventsToday.Count != 0)
                        todayEvents.ItemsSource = allEventsToday;
                    else {
                        allEventsToday.Add("Keine Veranstaltungen Heute");
                        todayEvents.ItemsSource = allEventsToday;
                    }
                    


                }

                if (eventAfter == nextEvent)
                {
                    SetNextMeeting( new Event(nextEvent.name, nextEvent.prof, nextEvent.room, nextEvent.dayOfWeek, nextEvent.hour.AddDays(7), nextEvent.end.AddDays(7)));
                }else
                {
                    SetNextMeeting(eventAfter);
                }
                
                threadTimer = new System.Threading.Timer(OnCallBack, null, 0, 100);
            }
            
        }

        /*
        private void ClearMainContent( )
        {

            nextEventCanvas.Visibility = Visibility.Visible;
        }

        private void ClearSecondContent( )
        {
            afterMeetingCanvas.Visibility = Visibility.Visible;
        }
        */

        private void OnCallBack( object state )
        {


            TimeSpan timeToEvent = nextEvent.hour - DateTime.Now;
            //code to check report 
            try {
                Dispatcher.Invoke(() => {


                    if (timeToEvent.TotalMinutes <= 0)
                    {
                        this.timer.Content = "Es läuft gerade";

                        if (eventList.IsEventOver(nextEvent))
                        {
                            nextEvent.UpdateDate();
                            eventList.SortByDate();

                        }
                    }
                    else if (timeToEvent.TotalHours < 1)
                    {
                        this.timer.Content = "" + timeToEvent.Minutes + " Minuten";
                    }else if(timeToEvent.TotalHours > 24)
                    {
                        this.timer.Content = ""+ (int)timeToEvent.TotalDays + "Tagen und " + (int)timeToEvent.Hours + " Stunden" ;
                    }
                    else
                    {
                        this.timer.Content = "" + (int)timeToEvent.TotalHours + " Stunden und " + timeToEvent.Minutes + " Minuten";
                    }
                });
            }catch( TaskCanceledException e )
            {
                Console.WriteLine( e.ToString( ) );
            }
           
        }

        private void SetTimerLabel(TimeSpan s)
        {
            timer.Content = s;
        }

        private TimeSpan GetTimeToEvent(Event ev)
        {

            DateTime now = DateTime.Now;
            /*
            DateTime dateOfEvent = eventList.GetEventDateTime(ev);
            
            TimeSpan resthour = dateOfEvent - now;      
            */
            return TimeSpan.FromSeconds(1);
        }

        private void SetMainContent(Event nextEvent)
        {

            label.Content = nextEvent.name;
            profLabel.Content = nextEvent.prof;
            roomLabel.Content = nextEvent.room;
            startLabel.Content = eventList.HourOutputWithoutSeconds(nextEvent.hour);
            endLabel.Content = eventList.HourOutputWithoutSeconds(nextEvent.end);

            var culture = new System.Globalization.CultureInfo("de-DE");

            dayLabel.Content = culture.DateTimeFormat.GetDayName(nextEvent.dayOfWeek);
        }

        private void SetNextMeeting(Event nextEvent) 
        {

            lbl_afterEventName.Content = nextEvent.name;
            lbl_afterEventProffesor.Content = nextEvent.prof;
            lbl_afterEventRoom.Content = nextEvent.room;
            if(nextEvent.dayOfWeek == DateTime.Now.DayOfWeek)
            {
                lbl_afterEventDay.Content = "Heute";
            }
            else
            {
                var culture = new System.Globalization.CultureInfo("de-DE");
                lbl_afterEventDay.Content = culture.DateTimeFormat.GetDayName(nextEvent.dayOfWeek);
            }
            
            lbl_afterEventHour.Content = eventList.HourOutputWithoutSeconds(nextEvent.hour);
        }

        private Note GetNoteByEventName( String name )
        {
            if ( name != null )
                        return null;

            return notesList.FindNoteByTitle( name ); 
        }

        private List<String> GetAllEventNamesToday( )
        {
            List<Event> events = eventList.GetList( );
            List<String> names = new List<String>( );
            DateTime today = DateTime.Now;

            if ( events.Count > 0 )
            {
                int max = events.Count;
                int start = 0;
                
                for( int i = events.Count - 1; i >= 0; i--)
                {
                    if( events[ i ].hour.DayOfYear == today.DayOfYear )
                    {
                        start = i;
                    }
                }

                max = max - ( max - start );
                for( int i = 0; i >= max; i++ )
                {
                    if ( events[ start ].hour.DayOfYear == today.DayOfYear )
                    {
                        names.Add( events[ start ].name );

                        if( start + 1 >= events.Count ) start = 0;
                        else start++;

                    }
                    else
                    {
                        break;
                    }
                }
            }

            return names;
        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {
            OnEventAddBtnClick( EventArgs.Empty );
        }

        public void OnEventAddBtnClick(EventArgs e)
        {
           
            EventHandler handler = EventAddBtnClick;

            if (handler != null)
            {
                handler(this, e);
                //Console.WriteLine("Event Invoke");
            }

        }
    }
}
