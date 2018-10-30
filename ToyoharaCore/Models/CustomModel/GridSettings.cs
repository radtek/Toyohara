using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToyoharaCore.Models.CustomModel
{
    public class GridSettings
    {   public bool? ColumnVisible { get; set; }
        public int? СolumnPosition { get; set; }
        public int? ColumnWidth { get; set; }
        public string ColumnRussianName { get; set; }
        public string ColumnName { get; set; }
        public GridSettings() { }
        public GridSettings(bool? columnVisible,int? columnWidth, int? columnPosition, string columnRussianName, string columnName) {
            this.ColumnVisible = columnVisible;
            this.ColumnWidth = columnWidth;
            this.СolumnPosition = columnPosition;
            this.ColumnRussianName = columnRussianName;
            this.ColumnName = columnName;
        }

    }
}
