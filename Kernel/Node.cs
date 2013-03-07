using System;
using System.Runtime.Serialization;

namespace Kernel
{
    //[Serializable]
    [DataContract]
    public class Node
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsSystem { get; set; }

        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }

        public override string ToString()
        {
            return ID + " - " + Name;
        }

    }
}
