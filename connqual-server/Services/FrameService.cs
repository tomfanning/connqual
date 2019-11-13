using shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace connqual_server.Services
{
    public class FrameService
    {
        private static List<(Frame, DateTime)> frames { get; set; } = new List<(Frame, DateTime)>();

        public List<(Frame Frame, DateTime Received)> Frames { get { return frames; } }
    }
}