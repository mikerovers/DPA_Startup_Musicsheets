using DPA_Musicsheets.Chain;
using DPA_Musicsheets.Commands.Open;
using DPA_Musicsheets.Managers;
using DPA_Musicsheets.State.Render;
using DPA_Musicsheets.State.Rendering;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using PSAMWPFControlLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DPA_Musicsheets.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private BlockContainer _blockContainer;
        private ChainOfResponsibility _chainOfResponsibility;
        private System.Collections.Generic.List<System.Windows.Input.Key> _keysDown;

        private string _fileName;
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                RaisePropertyChanged(() => FileName);
            }
        }

        public int CarrotPosition
        {
            get
            {
                return _blockContainer.CarotIndex;
            }
            set
            {
                _blockContainer.CarotIndex = value;
            }
        }

        /// <summary>
        /// The current state can be used to display some text.
        /// "Rendering..." is a text that will be displayed for example.
        /// </summary>
        private RenderingState _currentState;
        public RenderingState CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; RaisePropertyChanged(() => CurrentState); }
        }

        public MainViewModel(BlockContainer blockContainer)
        {
            // TODO: Can we use some sort of eventing system so the managers layer doesn't have to know the viewmodel layer?
            FileName = @"Files/Alle-eendjes-zwemmen-in-het-water.mid";
            _blockContainer = blockContainer;
            _blockContainer.CarotIndex = 0;
            _chainOfResponsibility = new ChainOfResponsibility();
            _chainOfResponsibility.AddHandlerToChain(new InsertTimeHandler());
            _chainOfResponsibility.AddHandlerToChain(new InsertTempoHandler());
            _chainOfResponsibility.AddHandlerToChain(new InsertClefHandler());
            _chainOfResponsibility.AddHandlerToChain(new OpenFileHandler());
            _chainOfResponsibility.AddHandlerToChain(new SaveAsPDFHandler());
            _chainOfResponsibility.AddHandlerToChain(new SaveAsLilypondHandler());
            _keysDown = new System.Collections.Generic.List<Key>();
        }

        public ICommand OpenFileCommand => new RelayCommand(() =>
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Midi or LilyPond files (*.mid *.ly)|*.mid;*.ly" };
            if (openFileDialog.ShowDialog() == true)
            {
                FileName = openFileDialog.FileName;
            }
        });

        public ICommand LoadCommand => new RelayCommand(() =>
        {
            var openFileCommand = new OpenFileCommand();
            openFileCommand.Execute(_blockContainer);
            _blockContainer.RenderState = new RenderingOnState();
            // _musicLoader.OpenFile(FileName);
        });

        #region Focus and key commands, these can be used for implementing hotkeys
        public ICommand OnLostFocusCommand => new RelayCommand(() =>
        {
            Console.WriteLine("Maingrid Lost focus");
        });

        public ICommand OnKeyDownCommand => new RelayCommand<KeyEventArgs>((e) =>
        {
            Console.WriteLine($"Key down: {e.Key}");
            _keysDown.Add(e.Key);
            if (_chainOfResponsibility.Handle(_keysDown, _blockContainer) != null)
            {
                e.Handled = true;
                _keysDown.Clear();
            }
        });

        public ICommand OnKeyUpCommand => new RelayCommand<KeyEventArgs> ((e) =>
        {
            Console.WriteLine("Key Up");
            _keysDown.Remove(e.Key);
        });

        public ICommand OnWindowClosingCommand => new RelayCommand(() =>
        {
            _blockContainer.textEditState.Exit();

            ViewModelLocator.Cleanup();
        });
        #endregion Focus and key commands, these can be used for implementing hotkeys
    }
}
    