using DPA_Musicsheets.Converter.Staffs;
using DPA_Musicsheets.Events;
using DPA_Musicsheets.Managers;
using GalaSoft.MvvmLight;
using PSAMControlLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DPA_Musicsheets.ViewModels
{
    public class StaffsViewModel : ViewModelBase
    {
        // These staffs will be bound to.
        public ObservableCollection<MusicalSymbol> Staffs { get; set; }
        private ToStaffsConverter _toStaffsConverter;
        private BlockContainer _blockContainer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="musicLoader">We need the musicloader so it can set our staffs.</param>
        public StaffsViewModel(BlockContainer blockContainer)
        {
            Staffs = new ObservableCollection<MusicalSymbol>();

            _toStaffsConverter = new ToStaffsConverter();
            _blockContainer = blockContainer;
            _blockContainer.TextChanged += HandleTextChanged;
        }

        private void HandleTextChanged(Object sender, TextChangedEventArgs args)
        {
            var symbols = _toStaffsConverter.ConvertTo(args.block);
            SetStaffs(symbols);
        }

        /// <summary>
        /// SetStaffs fills the observablecollection with new symbols. 
        /// We don't want to reset the collection because we don't want other classes to create an observable collection.
        /// </summary>
        /// <param name="symbols">The new symbols to show.</param>
        public void SetStaffs(IList<MusicalSymbol> symbols)
        {
            Staffs.Clear();
            foreach (var symbol in symbols)
            {
                Staffs.Add(symbol);
            }
        }
    }
}
