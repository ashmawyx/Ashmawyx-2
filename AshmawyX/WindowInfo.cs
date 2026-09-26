using System;

namespace AshmawyX
{
    public class WindowInfo
    {
        public IntPtr Handle { get; set; }
        public string Title { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
