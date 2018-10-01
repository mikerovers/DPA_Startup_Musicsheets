using DPA_Musicsheets.History;
using DPA_Musicsheets.Managers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DPA_Musicsheets.ViewModels
{
    public class LilypondViewModel : ViewModelBase
    {
        private MusicLoader _musicLoader;
        private MainViewModel _mainViewModel { get; set; }

        private HistoryOriginator _historyOriginator;
        private HistoryCareTaker _historyCareTaker;

        /// <summary>
        /// This text will be in the textbox.
        /// It can be filled either by typing or loading a file so we only want to set previoustext when it's caused by typing.
        /// </summary>
        public string LilypondText
        {
            get
            {
                return _historyOriginator.GetState();
            }
            set
            {
                _historyOriginator.SetState(value);
                RaisePropertyChanged(() => LilypondText);
            }
        }

        private bool _textChangedByLoad = false;
        private DateTime _lastChange;
        private static int MILLISECONDS_BEFORE_CHANGE_HANDLED = 1500;
        private bool _waitingForRender = false;

        public LilypondViewModel(MainViewModel mainViewModel, MusicLoader musicLoader)
        {
            // TODO: Can we use some sort of eventing system so the managers layer doesn't have to know the viewmodel layer and viewmodels don't know each other?
            // And viewmodels don't 
            _mainViewModel = mainViewModel;
            _musicLoader = musicLoader;
            _musicLoader.LilypondViewModel = this;
            
            this._historyCareTaker = new HistoryCareTaker();
            this._historyOriginator = new HistoryOriginator();
            LilypondText = "Please write your music here...";
        }

        public void LilypondTextLoaded(string text)
        {
            _textChangedByLoad = true;
            LilypondText = text;
            _historyCareTaker = new HistoryCareTaker();
            _textChangedByLoad = false;
        }

        /// <summary>
        /// This occurs when the text in the textbox has changed. This can either be by loading or typing.
        /// </summary>
        public ICommand TextChangedCommand => new RelayCommand<TextChangedEventArgs>((args) =>
        {
            // If we were typing, we need to do things.
            if (!_textChangedByLoad)
            {      
                _waitingForRender = true;
                _lastChange = DateTime.Now;

                _mainViewModel.CurrentState = "Rendering...";

                Task.Delay(MILLISECONDS_BEFORE_CHANGE_HANDLED).ContinueWith((task) =>
                {
                    if ((DateTime.Now - _lastChange).TotalMilliseconds >= MILLISECONDS_BEFORE_CHANGE_HANDLED)
                    {
                        _waitingForRender = false;
                        UndoCommand.RaiseCanExecuteChanged();

                        _musicLoader.LoadLilypondIntoWpfStaffsAndMidi(LilypondText);
                        _mainViewModel.CurrentState = "";

                        HistoryMomento tet = _historyOriginator.Save();
                        this._historyCareTaker.AddMomento(tet);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext()); // Request from main thread.
            }
        });

        #region Commands for buttons like Undo, Redo and SaveAs
        public RelayCommand UndoCommand => new RelayCommand(() =>
        {
            HistoryMomento h = _historyCareTaker.Undo();
            LilypondText = h.GetState();
        }, () => _historyCareTaker.CanUndo());

        public RelayCommand RedoCommand => new RelayCommand(() =>
        {
            HistoryMomento h = _historyCareTaker.Redo();
            LilypondText = h.GetState();
        }, () => _historyCareTaker.CanRedo());

        public ICommand SaveAsCommand => new RelayCommand(() =>
        {
            // TODO: In the application a lot of classes know which filetypes are supported. Lots and lots of repeated code here...
            // Can this be done better?
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "Midi|*.mid|Lilypond|*.ly|PDF|*.pdf" };
            if (saveFileDialog.ShowDialog() == true)
            {
                string extension = Path.GetExtension(saveFileDialog.FileName);
                if (extension.EndsWith(".mid"))
                {
                    _musicLoader.SaveToMidi(saveFileDialog.FileName);
                }
                else if (extension.EndsWith(".ly"))
                {
                    _musicLoader.SaveToLilypond(saveFileDialog.FileName);
                }
                else if (extension.EndsWith(".pdf"))
                {
                    _musicLoader.SaveToPDF(saveFileDialog.FileName);
                }
                else
                {
                    MessageBox.Show($"Extension {extension} is not supported.");
                }
            }
        });
        #endregion Commands for buttons like Undo, Redo and SaveAs
    }
}
