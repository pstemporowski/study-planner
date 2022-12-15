using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using log4net;
using log4net.Config;

namespace StudiumHelper
{
    
    class NotesList
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NotesList));
        public static List<Note> notesList;
        private static bool isCreated = false;
        public static bool isCreatedFirstTime = false;
        private static int standardWidth = 200;
        private static int standardHeight = 200;
        public readonly static String PATH_TO_NOTESJSON_DIRECTORY = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "StudiumHelper");
        public readonly static String PATH_TO_NOTESJSON_FILE = Path.Combine(PATH_TO_NOTESJSON_DIRECTORY, "noteList.json");
        public static Event nextEvent { get; set; }

        public static event EventHandler MemoChanged;
        public static event EventHandler MemoDeleted;

        public NotesList() {
            if ( !isCreated )
            {
                log.Info("Entering application.");
                notesList = new List<Note>();
                ReadJson();
                isCreated = true;
                //Console.WriteLine( notesList.Count );
                Note.MemoChangedInNote += MemoChangedInSomeNote;
            }
        }

        private void MemoChangedInSomeNote(object sender, EventArgs e)
        {
            OnMemoChanged(EventArgs.Empty);
            SaveJson();
        }
        //Throws Exception wenn ein Fehler auftritt.
        public void ReadJson()
        {
            try
            {
                log.Info("Greife gerade auf " + PATH_TO_NOTESJSON_FILE + " zu");
                string output = File.ReadAllText(PATH_TO_NOTESJSON_FILE);
                

                if (output.Length != 0)
                {
                    
                    notesList = JsonConvert.DeserializeObject<List<Note>>(output);
                    log.Info("Die NoteList besitzt " + notesList.Count + "Notitzen");
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                throw new Exception(e.ToString());

            }
            catch (Exception e)
            {
                Console.WriteLine( e.ToString( ));
                throw new Exception( e.ToString( ) );
                
            }
        }

        public void SaveJson()
        {
            try
            {
                var result = JsonConvert.SerializeObject( notesList );
                System.IO.File.WriteAllText( PATH_TO_NOTESJSON_FILE, result );
                log.Info("NoteList wurde gespeichert");
            }
            catch(Exception e)
            {

            }
            
        }

        public void AddNote( String title, double x, double y )
        {
            AddNote( title, x, y, 0 );
        }

        public void AddNote( String title, double x, double y, int zIndex ) 
        {

            notesList.Add( new Note(title, x, y, zIndex ));
            SaveJson();
        }

        public void AddNoteItem(Note note,  String noteContent)
        {
            if(note.title.Equals(nextEvent.name))
                
            note.AddMemo(noteContent);
            OnMemoChanged(EventArgs.Empty);

        }

        public void CreateStartNotes( List<Event> eventlist )
        {
            foreach( Event studyEvent in eventlist )
            {
                int[] xy = GetFreePosition();
                AddNote( studyEvent.name, xy[0], xy[1] );
            }
        }

        public void Remove(Note note)
        {
            if (note != null)
            {
                notesList.Remove(note);
            }

            if ( note.title.Equals(nextEvent.name) )
            {
                OnMemoChanged(EventArgs.Empty);
            }
            SaveJson();
            
        }

        public TextBox getTitleItemOfGrid( Grid grid )
        {
            StackPanel stackPanel = VisualTreeHelper.GetChild( grid, 1 ) as StackPanel;
            TextBox titleTxb = VisualTreeHelper.GetChild( stackPanel, 0 ) as TextBox;

            return titleTxb;
        }

        /*
        public void SetZIndex( String title )
        {
            Note myNote = notesList.Where( note => note.title.Equals( title )).FirstOrDefault( );
            
        }
        */

        public void setZIndex( Grid grid, int zIndex )
        {
            setZIndex(getTitleItemOfGrid( grid ).Text, zIndex);
        }

        public void setZIndex( String title, int zIndex )
        {
            
            Note myNote = notesList.Where( note => note.title.Equals( title )).FirstOrDefault( );
            if( myNote != null )
            {
                myNote.zIndex = zIndex;
            }

            SaveJson();
        }

        public void organizeZindex()
        {
            List<Note> SortedList = notesList.OrderBy(o => o.zIndex).ToList();
            int counter = 0;
            foreach(Note note in SortedList)
            {
                note.setZIndex(counter);
                counter++;
            }

            SaveJson();
        }

        public List< Note > getList( )
        {
            return notesList;
        }

        public int[ ] GetFreePosition( )
        {
            int[ ] output = new int[ 2 ];
            int x = 10;
            int y = 30;

            while ( CheckIfCollideStandardValues( x, y, notesList ))
            {
                if ( checkIfOut ( standardWidth + 30, x, 1100))
                {
                    y = y + standardHeight + 30;
                    x = 10;
                }else
                {
                    x += 30;
                }   
            }
            output[0] = x + 30;
            output[1] = y + 30;
            return output; 
        }

        public bool CheckIfCollide(int x, int y, int width, int height, List<Note> notesList)
        {
            foreach (Note note in notesList)
            {
                if (note.x < x + width &&
                note.x + standardWidth > x &&
                note.y < y + height &&
                note.y + height > y)
                {
                    return true;
                }
            }

            return false;
        }

        private bool checkIfOut(int width, int x, int pageLength)
        {
            if(x + width > pageLength || x < 0)
            {
                return true;
            }
            return false;
        }

        public bool CheckIfCollideStandardValues(int x, int y, List<Note> notesList)
        {
            foreach (Note note in notesList)
            {
                if (note.x < x + standardWidth &&
                note.x + standardWidth > x &&
                note.y < y + standardHeight &&
                note.y + standardHeight > y)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddEventsToNote(LinkedList<Event> eventList)
        {
            foreach (Event studyEvent in eventList)
            {
                int[] output = GetFreePosition();
                AddNote(studyEvent.name, output[0], output[1]);
            }
        }

        public Note FindNoteByTextBox(TextBox textBox)
        {
            return FindNoteByTitle(textBox.Text);
        }

        public Note FindNoteByTitle( String title )
        {

            Note returnNote = notesList.Find(note => note.title.Equals(title));   
            return returnNote; 
        }

        public void FindFreeName(String name)
        {

        }
        public void ChangeTitle(String oldTitle, String newTitle)
        {

            Note note = FindNoteByTitle(oldTitle);

            Console.WriteLine("Note gefunden " + (!(note is null)));
            if(note != null)
            {
                note.title = newTitle;
                OnMemoChanged(EventArgs.Empty);
            }
            SaveJson();
        }

        public void ChangePositionByTitle( String title, Point P )
        {
            ChangePositionByTitle( title, P.X, P.Y );
        }
        public void ChangePositionByTitle( String title, double x, double y )
        {
            Note note = FindNoteByTitle( title );
            if( note == null )
            {
                return;
            }
            note.x = x;
            note.y = y;
            SaveJson();
        }

        public void RemoveMemoInNote( Note note, String title)
        {
            if (note.title.Equals( nextEvent.name ))
            {
                OnMemoChanged(EventArgs.Empty);
            }
            note.RemoveSingleMemo(title);

            
            SaveJson();
        }

        public void OnMemoChanged(EventArgs e)
        {
            EventHandler handler = MemoChanged;

            if (handler != null)
            {
                handler(this, e);
                
            }
        }

        public void OnMemoDeleted(EventArgs e)
        {
            EventHandler handler = MemoDeleted;

            if (handler != null)
            {
                handler(this, e);
                Console.WriteLine("Gelöscht");
            }
        }

    }
}
