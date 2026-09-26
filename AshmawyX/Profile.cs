using System.Collections.Generic;

namespace AshmawyX
{
    public class Profile
    {
        public string Name { get; set; }
        public List<string> MirroredKeys { get; set; } = new List<string>();
        public double DelayFrom { get; set; }
        public double DelayTo { get; set; }
    }
}
