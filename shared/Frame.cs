using System;

namespace shared
{
    public class Frame
    {
        public int Sequence { get; set; }
        public Guid UniqueId { get; set; } = Guid.NewGuid();
        public DateTime Sent { get; set; }
    }
}
