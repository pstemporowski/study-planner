using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudiumHelper
{
    public class Note
    {
        public String title;
        public List<String> memoList = new List<String>();
        public double x;
        public double y;
        public int zIndex;
        public static EventHandler MemoChangedInNote;

        public Note()
        {
            
        }
        public Note( String title, double x, double y, int zIndex )
        {
            this.title = title;
            this.x = x;
            this.y = y;
            this.zIndex = zIndex;
        }

        public Note( String title, List<String> memoList, double x, double y, int zIndex )
        {
            this.title = title;
            this.memoList = memoList;
            this.x = x;
            this.y = y;
            this.zIndex = zIndex;

        }

        public Note( String title, List<String> notes )
        {
            this.title = title;
            this.memoList = notes;
        }

        public void setZIndex( int index )
        {
            this.zIndex = index;
        }

        public void AddMemo( String title )
        {
            memoList.Add( title );
            OnMemoChangedInNote(EventArgs.Empty);
        }

     
        public List<String> GetMemoList( )
        {
            return memoList;
        }
   
        public void ChangeMemo( String old, String changed )
        {

            for( int i = 0; i < memoList.Count; i++ )
            {
                if( memoList[i] == old )
                {
                    memoList[i] = changed;
                    OnMemoChangedInNote(EventArgs.Empty);
                    Console.WriteLine("MemoChanged");
                    return;
                }
            }
            
            Console.WriteLine( "Cannot find Memo" );
        }

        
        public bool RemoveSingleMemo( String input )
        {

            if (memoList.Remove(input))
            {
                OnMemoChangedInNote( EventArgs.Empty );
                Console.WriteLine( "Removed has been " + input );
                return true;
            }
            else return false;
                       
        }

        public void OnMemoChangedInNote(EventArgs e)
        {
            EventHandler handler = MemoChangedInNote;

            if (handler != null)
            {
                handler(this, e);
                //Console.WriteLine("Event Invoke");
            }
        }
    }
}
