using DPA_Musicsheets.Chain;
using DPA_Musicsheets.Commands;
using DPA_Musicsheets.Commands.Export;
using DPA_Musicsheets.Converter.Lilypond;
using DPA_Musicsheets.Managers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DPA_Musicsheets.ViewModels
{
    public class LilypondViewModel : ViewModelBase
    {
        private BlockContainer _blockContainer;
        private FromLilypondConverter _fromLilypondConverter;
        private ToLilypondConverter _toLilypondConverter;
        private MusicLoader _musicLoader;
        private MainViewModel _mainViewModel { get; set; }

        private bool _change;

        private string _text;
        private string _previousText;
        private string _nextText;

        public int CarrotPosition {
            get
            {
                return _blockContainer.CarotIndex;
            } set
            {
                _blockContainer.CarotIndex = value;
            }
        }

        /// <summary>
        /// This text will be in the textbox.
        /// It can be filled either by typing or loading a file so we only want to set previoustext when it's caused by typing.
        /// </summary>
        public string LilypondText
        {
            get
            {
                return _text;
            }
            set
            {
                if (!_waitingForRender && !_textChangedByLoad)
                {
                    _previousText = _text;
                }
                _text = value;
                RaisePropertyChanged(() => LilypondText);
            }
        }

        private bool _textChangedByLoad = false;
        private DateTime _lastChange;
        private static int MILLISECONDS_BEFORE_CHANGE_HANDLED = 1500;
        private bool _waitingForRender = false;

        public LilypondViewModel(MainViewModel mainViewModel, MusicLoader musicLoader, BlockContainer blockContainer)
        {
            _change = false;
            _fromLilypondConverter = new FromLilypondConverter();
            _toLilypondConverter = new ToLilypondConverter();

            _blockContainer = blockContainer;
            _blockContainer.TextChanged += (sender, args) =>
            {
                //if (_change)
               //{
                    _textChangedByLoad = true;
                    LilypondText = _toLilypondConverter.ConvertTo(args.block);
                    _textChangedByLoad = false;
                //}
                //_change = true;
            };
            // TODO: Can we use some sort of eventing system so the managers layer doesn't have to know the viewmodel layer and viewmodels don't know each other?
            // And viewmodels don't 
            _mainViewModel = mainViewModel;
            _musicLoader = musicLoader;
            _musicLoader.LilypondViewModel = this;
            
            _text = "Your lilypond text will appear here.";
        }

        public void LilypondTextLoaded(string text)
        {
            _textChangedByLoad = true;
            LilypondText = _previousText = text;
            _textChangedByLoad = false;
        }

        RelayCommand<RoutedEventArgs> _TextBoxSelectionChangedCommand = null;
        public ICommand TextBoxSelectionChangedCommand
        {
            get
            {
                if (_TextBoxSelectionChangedCommand == null)
                {
                    _TextBoxSelectionChangedCommand = new RelayCommand<RoutedEventArgs>((r) => TextBoxSelectionChanged(r), (r) => true);
                }

                return _TextBoxSelectionChangedCommand;
            }
        }

        protected virtual void TextBoxSelectionChanged(RoutedEventArgs _args)
        {
            CarrotPosition = (_args.OriginalSource as System.Windows.Controls.TextBox).SelectionStart;
        }

        public ICommand OnKeyDownCommand => new RelayCommand<KeyEventArgs>((e) =>
        {        

        });

        public ICommand OnKeyUpCommand => new RelayCommand<KeyEventArgs>((e) =>
        {
            
        });

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

                        _change = false;
                        _mainViewModel.CurrentState = "";
                        _blockContainer.Block = _fromLilypondConverter.ConvertTo(LilypondText);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext()); // Request from main thread.
            }
        });

        #region Commands for buttons like Undo, Redo and SaveAs
        public RelayCommand UndoCommand => new RelayCommand(() =>
        {
            _blockContainer.UndoBlock();
            //_nextText = LilypondText;
            //LilypondText = _previousText;
            //_previousText = null;
        }, () => _blockContainer.CanUndoBlock());

        public RelayCommand RedoCommand => new RelayCommand(() =>
        {
            //_previousText = LilypondText;
            //LilypondText = _nextText;
            //_nextText = null;
            _blockContainer.RedoBlock();
            RedoCommand.RaiseCanExecuteChanged();
        }, () =>  _blockContainer.CanRedoBlock());

        public ICommand SaveAsCommand => new RelayCommand(() =>
        {
            var saveAsCommand = new SaveAsCommand();
            saveAsCommand.Execute(_blockContainer);
        });
        #endregion Commands for buttons like Undo, Redo and SaveAs
    }
}
