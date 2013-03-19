using System.Collections.Generic;
using Kernel;

namespace Consulting
{
    public class WorkMemory
    {
        public List<Node> WorkedNodes { get; set; }
        public List<Arc> WorkedArcs { get; set; }

        public WorkMemory()
        {
            WorkedNodes = new List<Node>();
            WorkedArcs = new List<Arc>();
        }
    }
}
