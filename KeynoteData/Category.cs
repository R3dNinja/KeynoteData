using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace KeynoteData
{
    public class Category
    {
        public string Key { get; set; }
        public string CategoryTitle { get; set; }

        public Category(KeynoteEntry key)
        {
            this.Key = key.Key;
            this.CategoryTitle = key.KeynoteText;
        }
    }
}
