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
using RdClient.Shared.Navigation.Extensions;

namespace RdClient.Shared.CxWrappers
{

    public class ClipboardManager: IClipboardManager
    {
        private IDeferredExecution _deferredExecution;
        private IClipboardNotifier _notifier;

        static int XRESULT_SUCCESS = 0;
        static int XRESULT_FAIL = -1;

        public ClipboardManager(IDeferredExecution deferredExecution)
        {
            _deferredExecution = deferredExecution;
            _deferredExecution.Defer(
              () =>
              {
                  Clipboard.ContentChanged += new EventHandler<object>(OnLocalClipboardChanged);
              });

        }
        int IClipboardManager.GetClipboardFormats(out String[] formatsList, out int count)
        {
            AutoResetEvent are = new AutoResetEvent(false);
            IReadOnlyList<String> list = null;
            bool hasText = false;
            count = 0;
            formatsList = null;
           _deferredExecution.Defer(
             () =>
             {
                 DataPackageView dataPackageView = Clipboard.GetContent();
                 if (dataPackageView.Contains(StandardDataFormats.Text))
                 {
                     hasText = true;
                 }

                 are.Set();
             });
            
            are.WaitOne();
            if (hasText)
            {
                count = 1;
                formatsList = new String[1];
                formatsList[0] = "Text";
            }
            
            return XRESULT_SUCCESS;
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

        int IClipboardManager.ClearClipboard()
        {
            _deferredExecution.Defer(
               () =>
               {
                   Clipboard.Clear();
               });
            return XRESULT_SUCCESS;
        }

        int IClipboardManager.OnRemoteClipboardUpdated(String clipData)
        {
            Debug.WriteLine(clipData);

            _deferredExecution.Defer(
                () =>
                {
                    DataPackage dataPackage = new DataPackage();
                    dataPackage.SetText(clipData);
                    Clipboard.SetContent(dataPackage);
                });
            return XRESULT_SUCCESS;
         }

        int IClipboardManager.RegisterClipboard(IClipboardNotifier notifer)
        {
            _notifier = notifer;
            return XRESULT_SUCCESS;
        }



        void IClipboardManager.UnRegisterClipboard()
        {
            _notifier = null;
        }


        int IClipboardManager.GetClipboardData( out String clipData)
        {
            AutoResetEvent are = new AutoResetEvent(false);
            String clipDataBuffer = "";
            ClipboardManager that = this;

            int xRes = XRESULT_SUCCESS;
            _deferredExecution.Defer(
              async () =>
              {
                  DataPackageView dataPackageView = Clipboard.GetContent();
                  if (dataPackageView.Contains(StandardDataFormats.Text))
                  {
                      try
                      {
                          clipDataBuffer = await dataPackageView.GetTextAsync();
                          are.Set();
                      }
                      catch (Exception ex)
                      {
                          xRes = XRESULT_FAIL;
                          //RdTrace.IfFailXResultThrow("Error retrieving Text format from Clipboard: " + ex.Message);

                      }
                  }
              });

            are.WaitOne();
            clipData = clipDataBuffer;
            return xRes;
        }
    }
}
