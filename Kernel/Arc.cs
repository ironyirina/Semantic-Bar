using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kernel
{
    [DataContract]
    //[Serializable]
    public class Arc
    {
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Откуда
        /// </summary>
        [DataMember]
        public int From { get; set; }

        /// <summary>
        /// Куда
        /// </summary>
        [DataMember]
        public int To { get; set; }

        public override string ToString()
        {
            return Name;
        }

    }
}
