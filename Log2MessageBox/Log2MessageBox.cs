using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Vishnu.Interchange;
using System.Diagnostics;

namespace Log2MessageBox
{
    /// <summary>
    /// Loggt Vishnu-Ereignisse in eine MessageBox.
    /// </summary>
    /// <remarks>
    /// File: Log2MessageBox.cs
    /// Autor: Erik Nagel
    ///
    /// 26.05.2014 Erik Nagel: erstellt
    /// </remarks>
    public class Log2MessageBox : INodeLogger
    {
        #region INodeLogger implementaion

        /// <summary>
        /// Übernahme von diversen Logging-Informationen.
        /// </summary>
        /// <param name="loggerParameters">Spezifische Aufrufparameter oder null.</param>
        /// <param name="treeParameters">Für den gesamten Tree gültige Parameter oder null.</param>
        /// <param name="treeEvent">Klasse mit Informationen über das Ereignis.</param>
        /// <param name="additionalEventArgs">Enthält z.B. beim Event 'Exception' die zugehörige Exception.</param>
        public void Log(object loggerParameters, TreeParameters treeParameters, TreeEvent treeEvent, object additionalEventArgs)
        {
            //string logPath = null;
            //if (loggerParameters != null)
            //{
            //  logPath = loggerParameters.ToString();
            //}
            //string indent = "        ";
            string indent = "";
            //string delimiter = Environment.NewLine
            string delimiter = '"'.ToString() + " " + '"'.ToString();
            string addInfos = indent;
            if (treeEvent.Name.Contains("Exception"))
            {
                addInfos += (additionalEventArgs as Exception).Message;
            }
            if (treeEvent.Name.Contains("ProgressChanged"))
            {
                addInfos += String.Format("Fortschritt {0:d3}%", Convert.ToInt32((additionalEventArgs as object)));
            }
            string IdName = treeEvent.NodeName + "|" + treeEvent.SenderId;
            StringBuilder bigMessage = new StringBuilder("Knoten: " + IdName);
            bigMessage.Append(delimiter + treeEvent.Timestamp.ToString("yyyy-MM-dd HH:mm:ss,ms") + " Event: " + treeEvent.Name);
            bigMessage.Append(delimiter + indent + treeEvent.ReplaceWildcards("%MachineName%") + ", Thread: " + treeEvent.ThreadId.ToString());
            bigMessage.Append(", Tree: " + treeParameters.Name);
            if (treeEvent.SenderId != treeEvent.SourceId)
            {
                bigMessage.Append(", Quelle: " + treeEvent.SourceId);
            }
            if (addInfos.Trim() != "")
            {
                bigMessage.Append(delimiter + addInfos);
            }
            if (!String.IsNullOrEmpty(treeEvent.NodePath))
            {
                bigMessage.Append(delimiter + indent + treeEvent.NodePath);
            }
            bigMessage.Append(delimiter + indent + "Logical: " + treeEvent.Logical);
            bigMessage.Append(", Status: " + treeEvent.State.ToString());
            string workingDirectory = treeEvent.ReplaceWildcards("%WorkingDirectory%");
            bigMessage.Append(delimiter + indent + "WorkingDirectory: " + workingDirectory);
            bigMessage.Append(delimiter + indent + "Results: ");
            if (treeEvent.Results != null)
            {
                foreach (Result result in treeEvent.Results.Values)
                {
                    bigMessage.Append(delimiter + indent + result.ToString());
                }
            }
            bigMessage.Append(delimiter + indent + "Environment: ");
            if (treeEvent.Environment != null)
            {
                foreach (Result result in treeEvent.Environment.Values)
                {
                    bigMessage.Append(delimiter + indent + result.ToString());
                }
            }
            this.log('"'.ToString() + bigMessage.ToString() + '"'.ToString(), treeEvent);
        }

        #endregion INodeLogger implementaion

        /// <summary>
        /// Hier wird die MessageBox als eigene Exe geschleudert.
        /// </summary>
        /// <param name="message">Aus allen Parametern aufbereiteter string.</param>
        /// <param name="treeEvent">Klasse mit Informationen über das Ereignis.</param>
        private void log(string message, TreeEvent treeEvent)
        {
            Process nPad = new Process();
            nPad.StartInfo.FileName = treeEvent.GetResolvedPath("ConsoleMessageBox.exe");
            nPad.StartInfo.Arguments = "0 " + message + " " + "Knoten-Info";
            //InfoController.Say(String.Format("Rufe {0} mit {1}", nPad.StartInfo.FileName, nPad.StartInfo.Arguments));
            nPad.Start();
        }

    }
}
