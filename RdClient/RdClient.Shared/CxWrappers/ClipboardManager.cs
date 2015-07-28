using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using RdClient.Shared.Helpers;
using RdClientCx;
using System.Threading;

namespace RdClient.Shared.CxWrappers
{
    public class ClipboardManager: IClipboardManager
    {
        private IDeferredExecution _deferredExecution;
        private IClipboardNotifier _notifier;
        public ClipboardManager(IDeferredExecution deferredExecution)
        {
            _deferredExecution = deferredExecution;

            _deferredExecution.Defer(
              () =>
              {
                  Clipboard.ContentChanged += new EventHandler<object>(OnLocalClipboardChanged);
              });

        }

        private void OnLocalClipboardChanged(Object sender, Object e)
        {
            if(_notifier != null) { 
                _notifier.Notify();
            }
        }

        public static void Intialize(IDeferredExecution deferredExecution)
        {
            RdpConnectionStore rdpConnectionStoreCx;
            int xRes = RdpConnectionStore.GetConnectionStore(out rdpConnectionStoreCx);
            RdTrace.IfFailXResultThrow(xRes, "Unable to retrieve the connection store.");

            ClipboardManager manager = new ClipboardManager(deferredExecution);
            rdpConnectionStoreCx.SetClipboardManager(manager);
         }

        void IClipboardManager.OnRemoteClipboardUpdated(String clipData, int size)
        {
            Debug.WriteLine(clipData);
            Debug.WriteLine(size);

            _deferredExecution.Defer(
                () =>
                {
                    DataPackage dataPackage = new DataPackage();
                    dataPackage.SetText(clipData);
                    Clipboard.SetContent(dataPackage);
                });
         }

        void IClipboardManager.RegisterClipboard(IClipboardNotifier notifer)
        {
            _notifier = notifer;
        }



        void IClipboardManager.UnRegisterClipboard()
        {
            _notifier = null;
        }


        String IClipboardManager.GetClipboardData()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            String clipData = "";
            ClipboardManager that = this;
            _deferredExecution.Defer(
              async () =>
              {
                  DataPackageView dataPackageView = Clipboard.GetContent();
                  if (dataPackageView.Contains(StandardDataFormats.Text))
                  {
                      try
                      {
                          clipData = await dataPackageView.GetTextAsync();
                          are.Set();
                      }
                      catch (Exception ex)
                      {
                          //RdTrace.IfFailXResultThrow("Error retrieving Text format from Clipboard: " + ex.Message);

                      }
                  }
              });

            are.WaitOne();
            return clipData;
        }
    }
}
