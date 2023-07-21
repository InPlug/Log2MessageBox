using Vishnu.Interchange;

namespace Log2MessageBoxDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Log2MessageBox.Log2MessageBox cl = new Log2MessageBox.Log2MessageBox();
            cl.Log(null, new TreeParameters("MainTree", null),
              new TreeEvent("wasPassiert", "4711", "4712", "ne Knoden", "", false, NodeLogicalState.Done, null, null), null);
        }
    }
}