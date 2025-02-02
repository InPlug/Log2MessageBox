﻿using System;
using System.Text;
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
    /// 26.05.2014 Erik Nagel: Erstellt.
    /// 03.07.2021 Erik Nagel: Überarbeitet.
    /// 10.08.2023 Erik Nagel: Auf VishnuMessageBox umgestellt.
    /// </remarks>
    public class Log2MessageBox : INodeLogger
    {
        #region public members

        const string MessageNewLine = "#";

        #region INodeLogger implementaion

        /// <summary>
        /// Übernahme von diversen Logging-Informationen und Ausgabe in eine MessageBox.
        /// </summary>
        /// <param name="loggerParameters">Spezifische Aufrufparameter oder null.</param>
        /// <param name="treeParameters">Für den gesamten Tree gültige Parameter oder null.</param>
        /// <param name="treeEvent">Objekt mit Informationen über das Ereignis.</param>
        /// <param name="additionalEventArgs">Enthält z.B. beim Event 'Exception' die zugehörige Exception.</param>
        public void Log(object? loggerParameters, TreeParameters? treeParameters, TreeEvent treeEvent, object? additionalEventArgs)
        {
            // Zusammenbauen der zu loggenden Nachricht
            string bigMessage = BuildLogMessage(treeParameters, treeEvent, additionalEventArgs);

            this.WriteLog(bigMessage, treeEvent);
        }

        #endregion INodeLogger implementaion

        #endregion public members

        #region private members

        // Baut aus den übergebenen Parametern einen einzigen formatierten string zusammen.
        private string BuildLogMessage(TreeParameters? treeParameters, TreeEvent treeEvent, object? additionalEventArgs)
        {
            string indent = "";
            string addInfos = indent;
            if (treeEvent.Name.Contains("Exception"))
            {
                addInfos += ((Exception?)additionalEventArgs)?.Message;
            }
            if (treeEvent.Name.Contains("ProgressChanged"))
            {
                addInfos += String.Format("Fortschritt {0:d3}%", Convert.ToInt32((additionalEventArgs as object)));
            }
            string IdName = treeEvent.NodeName + MessageNewLine + treeEvent.SenderId;
            StringBuilder bigMessage = new StringBuilder("Knoten: " + IdName);
            bigMessage.Append(MessageNewLine + treeEvent.Timestamp.ToString("yyyy-MM-dd HH:mm:ss,ms")
                + " Event: " + treeEvent.Name);
            bigMessage.Append(MessageNewLine + indent + treeEvent.ReplaceWildcards("%MachineName%")
                + ", Thread: " + treeEvent.ThreadId.ToString());
            bigMessage.Append(", Tree: " + treeParameters?.Name);
            bigMessage.Append(", Quelle: " + treeEvent.SourceId);
            if (addInfos.Trim() != "")
            {
                bigMessage.Append(MessageNewLine + addInfos);
            }
            if (!String.IsNullOrEmpty(treeEvent.NodePath))
            {
                bigMessage.Append(MessageNewLine + indent + treeEvent.NodePath);
            }
            bigMessage.Append(MessageNewLine + indent + "Logical: " + treeEvent.Logical);
            bigMessage.Append(", Status: " + treeEvent.State.ToString());
            bigMessage.Append(MessageNewLine + indent + "WorkingDirectory: "
                              + treeEvent.ReplaceWildcards("%WorkingDirectory%"));
            bigMessage.Append(MessageNewLine + indent + "Results: ");
            if (treeEvent.Results != null)
            {
                foreach (Result? result in treeEvent.Results.Values)
                {
                    bigMessage.Append(MessageNewLine + indent + result?.ToString());
                }
            }
            bigMessage.Append(MessageNewLine + indent + "Environment: ");
            if (treeEvent.Environment != null)
            {
                foreach (Result? result in treeEvent.Environment.Values)
                {
                    bigMessage.Append(MessageNewLine + indent + result?.ToString());
                }
            }

            return '"'.ToString() + bigMessage.ToString() + '"'.ToString();
        }

        /// <summary>
        /// Hier wird die MessageBox als eigene Exe geschleudert.
        /// </summary>
        /// <param name="message">Aus allen Parametern aufbereiteter string.</param>
        /// <param name="treeEvent">Klasse mit Informationen über das Ereignis.</param>
        private void WriteLog(string message, TreeEvent treeEvent)
        {
            Process nPad = new Process();
            nPad.StartInfo.FileName = treeEvent.GetResolvedPath("VishnuMessageBox.exe");
            nPad.StartInfo.Arguments = "-EscalationCounter=0 -Message=" + message + " -Caption=" + "Knoten-Info"
                + " -MessageNewLine=" + MessageNewLine;
            nPad.Start();
        }

        #endregion private members

    }
}
