using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.Revit.DB;

namespace KeynoteData
{
    public partial class AddNote : System.Windows.Forms.Form
    {
        public string ParentKey { get; set; }
        public string Key { get; set; }
        public string KeyNoteText { get; set; }
        public string SpecSection { get; set; }
        public int keyEnd { get; set; }
        public string FullKeyText { get; set; }
        public KeynoteEntry FullKey { get; set; }

        public AddNote(string pKey, int lastKey)
        {
            InitializeComponent();
            this.ParentKey = pKey;
            this.keyEnd = lastKey + 1;
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbKeyNoteText.Text))
            {
                //TaskDialog.Show("Keynote", "Category Code cannot be empty");
                MessageBox.Show("Keynote cannot be empty");
            }
            else
            {
                this.KeyNoteText = tbKeyNoteText.Text;
                if (tbSpecSection.Text != "")
                {
                    //Spec
                    this.SpecSection = tbSpecSection.Text;
                    this.FullKeyText = this.KeyNoteText + "-" + this.SpecSection;
                }
                else
                {
                    this.FullKeyText = this.KeyNoteText;
                }
                this.Key = this.ParentKey + "." + this.keyEnd.ToString("D2");

                KeynoteEntry newKey = new KeynoteEntry(this.Key, this.ParentKey, this.FullKeyText);
                this.FullKey = newKey;

                Command.keynotes.Add(new KeyNote(newKey));
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

        }
    }
}
