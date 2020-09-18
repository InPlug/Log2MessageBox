using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Vishnu.Interchange;

namespace Log2MessageBox
{
    class Program
    {
        static void Main(string[] args)
        {
            Log2MessageBox cl = new Log2MessageBox();
            cl.Log(null, new TreeParameters("MainTree", null),
              new TreeEvent("wasPassiert", "4711", "4712", "ne Knoden", "", false, NodeLogicalState.Done, null, null), null);
        }
    }
}
