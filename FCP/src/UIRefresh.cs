using System;
using System.Threading;
using System.Threading.Tasks;
using FCP.Models;
using FCP.ViewModels;
using Helper;
using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.Messaging;
using FCP.src.MessageManager;
using FCP.src.Enum;
using FCP.src.Dictionary;

namespace FCP.src
{
    class UIRefresh
    {
        public MainUILayoutModel UILayout { get => _uiLayoutModel; set => _uiLayoutModel = value; }
        private CancellationTokenSource _cts;
        private MainUILayoutModel _uiLayoutModel;

        public async void StartRefreshAsync()
        {
            _cts = new CancellationTokenSource();
            while (!_cts.IsCancellationRequested)
            {
                await Task.Delay(500);
                try
                {
                    WeakReferenceMessenger.Default.Send(new CommandMessage(), nameof(eCommandCollection.SetSimpleWindowPosition));
                    WeakReferenceMessenger.Default.Send(new UpdateMainUIChangeMessage(UILayout));
                }
                catch (Exception ex)
                {
                    LogService.Exception(ex);
                    MsgCollection.ShowDialog(ex, "錯誤", PackIconKind.Error, dColor.GetSolidColorBrush(eColor.Red));
                }
            }
            _cts = null;
        }

        public void StopRefresh()
        {
            if (_cts != null)
            {
                _cts.Cancel();
            }
        }
    }
}
