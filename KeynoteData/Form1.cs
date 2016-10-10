using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Data;

using Autodesk.Revit.DB;

namespace KeynoteData
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public string tempFilePath { get; set; }
        DataTable dt = new DataTable();
        BindingSource bs = new BindingSource();
        BindingSource bS = new BindingSource();
        BindingList<Category> blist;
        BindingList<KeyNote> myKeyNoteList = new BindingList<KeyNote>(Command.keynotes.ToList());
        public AddNote addDialog;
        public AddCategory addCatDialog;
        //bs.DataSource = new BindingList<KeyNote>(Command.keynotes.ToList());

        public Form1()
        {
            InitializeComponent();
            bindComboList();
            bindGrid();
            
        }

        private void bindComboList()
        {
            blist = new BindingList<Category>();
            foreach (Category c in Command.categories)
            {
                blist.Add(c);
            }

            bS = new BindingSource();
            bS.DataSource = blist;

            comboBox1.DataSource = bS;
            comboBox1.DisplayMember = "CategoryTitle";
            comboBox1.ValueMember = "Key";
        }

        private void bindCombo()
        {
            //BindingSource bS = new BindingSource();
            bS.DataSource = Command.categories;
            comboBox1.DataSource = bS.DataSource;
            comboBox1.DisplayMember = "CategoryTitle";
            comboBox1.ValueMember = "Key";
        }

        private void bindGrid()
        {
            //foreach (KeyNote note in Command.keynotes)
            //{
            //    myKeyNoteList.Add(note);
            //}
            //bs.DataSource = myKeyNoteList;
            //dataGridView1.DataSource = myKeyNoteList;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnsMode.Fill;
            //dataGridView1.AutoResizeColumns();
            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;


            BindingList<KeyNote> filtered = new BindingList<KeyNote>(myKeyNoteList.Where(obj => obj.ParentKey.Equals(comboBox1.SelectedValue)).ToList());

            dataGridView1.DataSource = filtered;
            dataGridView1.Update();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindingList<KeyNote> filtered = new BindingList<KeyNote>(myKeyNoteList.Where(obj => obj.ParentKey.Equals(comboBox1.SelectedValue)).ToList());

            dataGridView1.DataSource = filtered;
            dataGridView1.Update();
        }

        private void AddNote_Click(object sender, EventArgs e)
        {
            //determine last note in list
            BindingList<KeyNote> filtered = new BindingList<KeyNote>(myKeyNoteList.Where(obj => obj.ParentKey.Equals(comboBox1.SelectedValue)).ToList());
            List<KeyNote> list = filtered.ToList();
            //int lastNote = FindMaxKey(list);
            ShowAddForm(FindMaxKey(list));

            filtered = new BindingList<KeyNote>(myKeyNoteList.Where(obj => obj.ParentKey.Equals(comboBox1.SelectedValue)).ToList());
            list = filtered.ToList();
            dataGridView1.DataSource = filtered;
            dataGridView1.Update();
        }

        private void AddCategory_Click(object sender, EventArgs e)
        {
            ShowAddCategoryForm();
        }

        private void DeleteNote_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell oneCell in dataGridView1.SelectedCells)
            {
                if (oneCell.Selected)
                {
                    //on
                    var temp = oneCell.Value;
                    var rIndex = oneCell.RowIndex;
                    var tempText = dataGridView1.Rows[rIndex].Cells[2].Value;
                    var ss = dataGridView1.Rows[rIndex].Cells[3].Value;
                    if (ss != "")
                    {
                        var fullTemp = tempText + "-" + ss;
                    }
                    else
                    {
                        var fullTemp = tempText;
                    }
                    var index = Command.keynotes.FindIndex(a => a.KeyNoteText == tempText);
                    if (index > -1)
                    {
                        Command.keynotes.RemoveAt(index);
                    }
                    dataGridView1.Rows.RemoveAt(oneCell.RowIndex);
                    var itemToRemove = myKeyNoteList.Where(a => a.KeyNoteText == tempText).ToList();
                    foreach (var item in itemToRemove)
                    {
                        myKeyNoteList.Remove(item);
                    }
                }
            }
        }

        private void ShowAddCategoryForm()
        {
            addCatDialog = new AddCategory();
            var result = addCatDialog.ShowDialog();
            
            if (result == DialogResult.OK)
            {
                bindComboList();
                /*KeynoteEntry addedCategory = addCatDialog.FullCategory;

                //this.comboBox1.Items.Clear();
                //this.comboBox1.DataSource = Command.categories;

                bS.DataSource = Command.categories;
                comboBox1.DataSource = bS.DataSource;
                comboBox1.DisplayMember = "CategoryTitle";
                comboBox1.ValueMember = "Key";*/

            }
        }

        private void ShowAddForm(int lastNote)
        {
            addDialog = new AddNote(comboBox1.SelectedValue.ToString(), lastNote);
            var result = addDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                KeynoteEntry addedKey = addDialog.FullKey;
                myKeyNoteList.Add(new KeyNote(addedKey));
            }
            
        }

        private int FindMaxKey(List<KeyNote> list)
        {
            if (list.Count == 0)
            {
                //empty list
            }
            int maxNote = 0;
            foreach (KeyNote note in list)
            {
                if (note.KeyEnd > maxNote)
                {
                    maxNote = note.KeyEnd;
                }
            }
            return maxNote;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {

        }

        private void Finialize_Click(object sender, EventArgs e)
        {
            var builder = new StringBuilder();
            foreach (Category category in Command.categories)
            {
                builder.Append(category.Key.ToString());
                builder.Append("\t");
                builder.Append(category.CategoryTitle.ToString());
                builder.Append(Environment.NewLine);
                foreach (KeyNote key in Command.keynotes)
                {
                    if (key.ParentKey == category.Key)
                    {
                        builder.Append(key.Key.ToString());
                        if (key.SpecSection != "")
                        {
                            if (key.SpecSection != null)
                            {
                                builder.Append("\t");
                                builder.Append(key.KeyNoteText.ToString());
                                builder.Append("-");
                                builder.Append(key.SpecSection.ToString());
                            }
                            else
                            {
                                builder.Append("\t");
                                builder.Append(key.KeyNoteText.ToString());
                            }

                        }
                        else
                        {
                            builder.Append("\t");
                            builder.Append(key.KeyNoteText.ToString());
                        }
                        builder.Append("\t");
                        builder.Append(key.ParentKey.ToString());
                        builder.Append(Environment.NewLine);
                    }

                }
            }
            var filePath = Path.GetTempFileName();
            tempFilePath = filePath;
            var file = new FileStream(filePath, FileMode.Create);
            var writer = new StreamWriter(file);
            writer.Write(builder.ToString());
            writer.Flush();
            writer.Close();
            file.Close();
        }
    }
}
