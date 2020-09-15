using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplRad
{
    public class StegoOptions
    {
        internal bool InsertMessageSizeAtBeginning
        {
            get;
            set;
        }

        internal bool LoopMessage
        {
            get;
            set;
        }

        internal bool CompressMessage
        {
            get;
            set;
        }

        internal bool EncryptMessage
        {
            get;
            set;
        }
        internal int LsbUsed
        {
            get;
            set;
        }

        public StegoOptions(bool loopMessage, bool compressMessage, bool encryptMessage, int lsbUsed, bool insertMessageSizeAtBeginning = true)
        {
            this.LoopMessage = loopMessage;
            this.CompressMessage = compressMessage;
            this.EncryptMessage = encryptMessage;
            this.LsbUsed = lsbUsed;
            this.InsertMessageSizeAtBeginning = insertMessageSizeAtBeginning;
        }
    }
}
