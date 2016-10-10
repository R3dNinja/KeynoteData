using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace KeynoteData
{
    public class KeyNote
    {
        public string ParentKey { get; set; }
        public string Key { get; set; }
        public string KeyNoteText { get; set; }
        public string SpecSection { get; set; }
        public int KeyEnd { get; set; }
        //public bool IsACategory { get; set; }

        public KeyNote(KeynoteEntry key)
        {
            this.Key = key.Key;
            this.ParentKey = key.ParentKey;
            KeyEndExtraction(this.Key);
            KeyNoteTextExtraction(key.KeynoteText);
        }

        private void KeyEndExtraction(string key)
        {
            //adds a number that I can get a value from
            int index = key.IndexOf(".");
            string keyEnd = key.Substring(index + 1);
            try
            {
                this.KeyEnd = Int32.Parse(keyEnd);
            }
            catch
            {
                this.KeyEnd = 0;
            }
        }

        private void KeyNoteTextExtraction(string KeyNoteText)
        {
            bool specSectionFound = SpecSectionSearch(KeyNoteText);
            if (specSectionFound == false)
            {
                SearchForRE(KeyNoteText);
            }
        }

        private bool SpecSectionSearch(string KeyNoteText)
        {
            System.Text.RegularExpressions.Regex pattern = new System.Text.RegularExpressions.Regex(@"(\d{6})");
            System.Text.RegularExpressions.Match match = pattern.Match(KeyNoteText);
            string specSection = match.ToString();
            int index = match.Index;
            if (specSection.Length == 6)
            {
                char c = KeyNoteText[index - 1];
                string dash = "-";
                char compare = dash[0];
                if (c == compare)
                {
                    this.KeyNoteText = KeyNoteText.Substring(0, index - 1);
                }
                else
                {
                    this.KeyNoteText = KeyNoteText.Substring(0, index - 0);
                }
                this.SpecSection = specSection;
                return true;
            }
            else return false;
        }

        private void SearchForRE(string KeyNoteText)
        {
            int lastIndex = KeyNoteText.Length;
            System.Text.RegularExpressions.Regex pattern = new System.Text.RegularExpressions.Regex(@"(RE:)");
            System.Text.RegularExpressions.Match match = pattern.Match(KeyNoteText);
            string specSection = match.ToString();
            int index = match.Index;
            if (specSection == "RE:")
            {
                this.SpecSection = KeyNoteText.Substring(index, lastIndex - index);
                char c = KeyNoteText[index - 1];
                string dash = "-";
                char compare = dash[0];
                if (c == compare)
                {
                    this.KeyNoteText = KeyNoteText.Substring(0, index - 1);
                }
                else
                {
                    this.KeyNoteText = KeyNoteText.Substring(0, index - 0);
                }
            }
            else
            {
                this.KeyNoteText = KeyNoteText;
            }
        }
    }
}
