using System;
using System.IO;

namespace ATA.EMMExemptions
{
    internal class OutputLog : IDisposable
    {
        private StreamWriter _writer;

        public OutputLog()
        {
            this._writer = new StreamWriter("EMMExemptions_" + (object)DateTime.Now.Day + (object)DateTime.Now.Hour + (object)DateTime.Now.Minute + ".txt");
        }

        public void LogError(string error)
        {
            this._writer.WriteLine(error);
        }

        public void LogInfo(string info)
        {
            this._writer.WriteLine(info);
        }

        public void Dispose()
        {
            if (this._writer == null)
                return;
            try
            {
                this._writer.Flush();
                this._writer.Close();
                this._writer.Dispose();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
