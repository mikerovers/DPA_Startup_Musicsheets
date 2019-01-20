
using DPA_Musicsheets.Converter;
using DPA_Musicsheets.Converter.Lilypond;
using DPA_Musicsheets.Converter.Midi;
using DPA_Musicsheets.Converter.Staffs;
using DPA_Musicsheets.Converter.Token;
using DPA_Musicsheets.Models;
using DPA_Musicsheets.ViewModels;
using PSAMControlLibrary;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Clef = PSAMControlLibrary.Clef;
using ClefType = PSAMControlLibrary.ClefType;
using Note = PSAMControlLibrary.Note;
using Rest = PSAMControlLibrary.Rest;
using TimeSignature = PSAMControlLibrary.TimeSignature;

namespace DPA_Musicsheets.Managers
{
    /// <summary>
    /// This is the one and only god class in the application.
    /// It knows all about all file types, knows every viewmodel and contains all logic.
    /// TODO: Clean this class up.
    /// </summary>
    public class MusicLoader
    {
        #region Properties
        public string LilypondText { get; set; }
        public List<MusicalSymbol> WPFStaffs { get; set; } = new List<MusicalSymbol>();
        private static List<Char> notesorder = new List<Char> { 'c', 'd', 'e', 'f', 'g', 'a', 'b' };

        public Sequence MidiSequence { get; set; }
        #endregion Properties

        private int _beatNote = 4;    // De waarde van een beatnote.
        private int _bpm = 120;       // Aantal beatnotes per minute.
        private int _beatsPerBar;     // Aantal beatnotes per maat.

        public MainViewModel MainViewModel { get; set; }
        public LilypondViewModel LilypondViewModel { get; set; }
        public MidiPlayerViewModel MidiPlayerViewModel { get; set; }
        public StaffsViewModel StaffsViewModel { get; set; }

        /// <summary>
        /// Opens a file.
        /// TODO: Remove the switch cases and delegate.
        /// TODO: Remove the knowledge of filetypes. What if we want to support MusicXML later?
        /// TODO: Remove the calling of the outer viewmodel layer. We want to be able reuse this in an ASP.NET Core application for example.
        /// </summary>
        /// <param name="fileName"></param>
        public void OpenFile(string fileName)
        {
            var fromConverterFactory = new FromConverterFactory();
            var converter = fromConverterFactory.GetConverter(Path.GetExtension(fileName));

            if (converter == null)
            {
                throw new NotSupportedException($"File extension {Path.GetExtension(fileName)} is not supported.");
            }

            Block block = converter.ConvertToFromFile(fileName);
            var toLilypondConverter = new ToLilypondConverter();
            string output = toLilypondConverter.ConvertTo(block);
            this.LilypondText = output;
            this.LilypondViewModel.LilypondTextLoaded(this.LilypondText);

            LoadLilypondIntoWpfStaffsAndMidi(LilypondText);

            return;
        }

        /// <summary>
        /// This creates WPF staffs and MIDI from Lilypond.
        /// TODO: Remove the dependencies from one language to another. If we want to replace the WPF library with another for example, we have to rewrite all logic.
        /// TODO: Create our own domain classes to be independent of external libraries/languages.
        /// </summary>
        /// <param name="content"></param>
        public void LoadLilypondIntoWpfStaffsAndMidi(string content)
        {
            LilypondText = content;
            content = content.Trim().ToLower().Replace("\r\n", " ").Replace("\n", " ").Replace("  ", " ");
            var toTokenConverter = new ToTokenConverter();
            LinkedList<LilypondToken> tokens = toTokenConverter.GetTokensFromLilypond(content);
            WPFStaffs.Clear();
        }

        #region Midi loading (loads midi to lilypond)

        #endregion Midiloading (loads midi to lilypond)

        #region Staffs loading (loads lilypond to WPF staffs)

        #endregion Staffs loading (loads lilypond to WPF staffs)

        #region Saving to files
        
        #endregion Saving to files
    }
}
