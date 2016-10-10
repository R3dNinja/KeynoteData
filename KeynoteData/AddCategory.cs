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
    public partial class AddCategory : System.Windows.Forms.Form
    {

        public string KeynoteCategory { get; set; }
        public string CategoryCode { get; set; }
        public KeynoteEntry FullCategory { get; set; }


        public AddCategory()
        {
            InitializeComponent();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbCategoryText.Text))
            {
                //TaskDialog.Show("Keynote", "Category Code cannot be empty");
                MessageBox.Show("Category cannot be empty");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(tbCategoryCode.Text))
                {
                    //TaskDialog.Show("Keynote", "Category Code cannot be empty");
                    MessageBox.Show("Category Code cannot be empty");
                }
                else
                {
                    bool catCodeExists = false;
                    foreach (Category c in Command.categories)
                    {
                        if (c.Key == tbCategoryCode.Text)
                        {
                            catCodeExists = true;
                        }
                    }
                    if (catCodeExists == false)
                    {
                        this.KeynoteCategory = tbCategoryText.Text;
                        this.CategoryCode = tbCategoryCode.Text;

                        KeynoteEntry newCategory = new KeynoteEntry(this.CategoryCode, this.KeynoteCategory);
                        Command.categories.Add(new Category(newCategory));

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Category Code is already in use. \n Please chose a unique Category Code.");
                    }
                }
            }
        }
    }
}
