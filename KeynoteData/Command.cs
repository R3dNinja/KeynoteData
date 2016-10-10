#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace KeynoteData
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Form1 dialog;
        public static List<KeyNote> keynotes = new List<KeyNote>();
        public static List<Category> categories = new List<Category>();
        public Document docu { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            this.docu = doc;

            var keyTable = KeynoteTable.GetKeynoteTable(doc);
            var keys = keyTable.GetKeyBasedTreeEntries();
            keynotes = new List<KeyNote>();
            categories = new List<Category>();

            foreach (KeynoteEntry key in keys)
            {
                if (key.ParentKey == "")
                {
                    categories.Add(new Category(key));
                }
                else
                {
                    keynotes.Add(new KeyNote(key));
                }
            }
            ShowForm();

            return Result.Succeeded;
        }

        private void ShowForm()
        {
            dialog = new Form1();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ReloadNotes();
            }
            else
            {
            }
            dialog.Dispose();
        }

        public void ReloadNotes()
        {
            KeynoteTable knTable = KeynoteTable.GetKeynoteTable(this.docu);
            KeyBasedTreeEntries kntableEntries = knTable.GetKeyBasedTreeEntries();
            ModelPath p = ModelPathUtils.ConvertUserVisiblePathToModelPath(dialog.tempFilePath);
            KeyBasedTreeEntriesLoadResults loadResults = new KeyBasedTreeEntriesLoadResults();

            ExternalResourceReference s = ExternalResourceReference.CreateLocalResource(this.docu, ExternalResourceTypes.BuiltInExternalResourceTypes.KeynoteTable, p, PathType.Absolute);
            Transaction t = new Transaction(this.docu, "Reload");
            t.Start();
            knTable.LoadFrom(s, loadResults);
            t.Commit();
            //ExternalResourceReference exRef = new ExternalResourceReference(
            //Transaction tr = new Transaction(this.docu, "Reload");
            //tr.Start();
        }
    }
}
