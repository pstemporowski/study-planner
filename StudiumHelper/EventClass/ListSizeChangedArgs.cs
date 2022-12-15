using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudiumHelper.EventClass
{
    public class ListSizeChangedArgs : EventArgs
    {
        public ListSizeChangedArgs(int listSize)
        {
            this.listSize = listSize;
        }

        public int listSize
        {
            get;
            private set;
        }
    }
}
