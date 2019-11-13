using System;

namespace shared
{
    public class WrappedFrame
    {
        public Frame Frame { get; set; }
        public DateTime Received { get; set; }
        public double DelayMs => (Received - Frame.Sent).TotalMilliseconds;
        public WrappedFrame(Frame frame, DateTime received)
        {
            this.Frame = frame;
            this.Received = received;
        }
    }
}
