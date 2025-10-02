using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GUI
{
    public class GuiEvent: UserControl
    {
        // Event class for the PathChanged event
        public class PathChangedEventArgs : EventArgs
        {
            public IList<string> ThisSourcePaths { get; private set; }

            // Constructor
            public PathChangedEventArgs(IList<string> sourcePaths)
            {
                ThisSourcePaths = sourcePaths;
            }
        }

        // By using the generic EventHandler<T> event type 
        // there is not need to declare a separate delegate type. 
        public event EventHandler<PathChangedEventArgs> PathChanged;

        //The event-invoking method that derived classes can override. 
        protected void OnPathChanged(PathChangedEventArgs e)
        {
            EventHandler<PathChangedEventArgs> handler = PathChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}