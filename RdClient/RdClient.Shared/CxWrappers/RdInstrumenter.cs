using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;
using Windows.Storage.Search;

namespace RdClient.Shared.CxWrappers
{
    public class RdInstrumentation
    {
        private string _message;
        public string Message { get { return _message; } }

        private DateTime _timestamp;
        public DateTime TimeStamp { get { return _timestamp; } }

        public RdInstrumentation(string message)
        {
            _message = message;
            _timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return TimeStamp + " " + Message + Environment.NewLine;
        }
    }

    public class RdInstrumenter
    {
        private ICollection<RdInstrumentation> _log = new List<RdInstrumentation>();

        private StorageFolder _rootFolder;
        public StorageFolder RootFolder
        {
            get
            {
                if(_rootFolder == null)
                {
                    StorageFolder root = null;
                    ApplicationData.Current.LocalFolder.GetOrCreateFolderAndCall("instrumentation", folder => root = folder);
                    _rootFolder = root;
                }

                return _rootFolder;
            }
        }

        private Stream _stream;
        public Stream WriteStream
        {
            get
            {
                if(_stream == null)
                {
                    Task<Stream> task = RootFolder.OpenStreamForWriteAsync("instrument.txt", CreationCollisionOption.OpenIfExists);
                    task.Wait();
                    _stream = task.Result;
                }
                return _stream;
            }
        }

        private bool _enabled = false;

        public RdInstrumenter(bool enabled = false)
        {
            _enabled = enabled;
        }

        public void Instrument(string message)
        {
            if(_enabled)
            {
                RdInstrumentation instrumentation = new RdInstrumentation(message);
                _log.Add(instrumentation);

                byte[] msg = Encoding.UTF8.GetBytes(instrumentation.ToString());
                var ignore = WriteStream.WriteAsync(msg, 0, msg.Length);
                WriteStream.Flush();
            }
        }
    }
}
