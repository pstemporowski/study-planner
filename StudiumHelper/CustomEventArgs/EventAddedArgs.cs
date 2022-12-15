using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudiumHelper.CustomEventArgs
{
    public class EventAddedArgs : EventArgs
    {
        private Event addedEvent;
        public EventAddedArgs(Event addedEvent)
        {
            this.addedEvent = addedEvent;
        } // eo ctor

        public Event Data { get { return addedEvent; } }
    }
}
