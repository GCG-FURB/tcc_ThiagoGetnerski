using System;
using System.Windows;
using System.Windows.Input;

namespace i1Sharp
{
    class TemporaryCursor : IDisposable
    {
        private FrameworkElement parent;
        private Cursor originalCursor;
        
        public TemporaryCursor (FrameworkElement parent, Cursor cursor)
        {
            this.parent = parent;
            this.originalCursor = parent.Cursor;
            this.parent.Cursor = cursor;
        }
        
        public void Dispose()
        {
            this.parent.Cursor = originalCursor;
        }
    }
}
