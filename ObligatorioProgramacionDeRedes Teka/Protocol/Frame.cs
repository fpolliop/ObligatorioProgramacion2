using DataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public class Frame
    {
        public ActionType Action { get; set; }
        public Byte[] Data { get; set; }
        public int DataLength { get; set; }

        public Frame()
        {

        }

        public Frame(ActionType action, Byte[] data)
        {
            Action = action;
            Data = data;
            DataLength = data.Length;
        }
    }
}
