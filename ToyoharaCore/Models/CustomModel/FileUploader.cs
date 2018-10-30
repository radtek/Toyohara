using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToyoharaCore.Models.CustomModel
{
    public class FileUploader
    {
        public FileUploader(string Id, string UploadFileAccept, string UploadURL, string Name, string UploadUrlClass, string FlowWindowName, string InsertProcParam, string SelectProcParam,
            string InsertProc, string SelectProc, string InsertTemplate, string SelectTemplate, int LoadingTypeId, string Description,
            string Summary, int ColumnCountSelect, int RowCountSelect, int ColumnCount, int RowCount, int WorkSheetNumber, string RussianFormName, string OnCommitSuccessFunction = "OnCommitSuccess"
            )
        {
            this.Id = Id;
            this.UploadURL = UploadURL;
            this.Name = Name;
            this.FlowWindowName = FlowWindowName;
            this.InsertProcParam = InsertProcParam;
            this.SelectProcParam = SelectProcParam;
            this.InsertProc = InsertProc;
            this.SelectProc = SelectProc;
            this.InsertProcParam = InsertProcParam;
            this.SelectProcParam = SelectProcParam;
            this.InsertProc = InsertProc;
            this.SelectProc = SelectProc;
            this.InsertTemplate = InsertTemplate;
            this.SelectTemplate = SelectTemplate;
            this.LoadingTypeId = LoadingTypeId;
            this.Description = Description;
            this.Summary = Summary;
            this.ColumnCountSelect = ColumnCountSelect;
            this.RowCountSelect = RowCountSelect;
            this.ColumnCount = ColumnCount;
            this.RowCount = RowCount;
            this.WorkSheetNumber = WorkSheetNumber;
            this.UploadFileAccept = UploadFileAccept;
            this.RussianFormName = RussianFormName;
            this.OnCommitSuccessFunction = OnCommitSuccessFunction;
        }
        public string Id { get; set; }
        public string UploadURL { get; set; }
        public string Name { get; set; }
        public string UploadUrlClass { get; set; }
        public string FlowWindowName { get; set; }
        public string InsertProcParam { get; set; }
        public string SelectProcParam { get; set; }
        public string InsertProc { get; set; }
        public string SelectProc { get; set; }
        public string InsertTemplate { get; set; }
        public string SelectTemplate { get; set; }
        public int LoadingTypeId { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public int ColumnCountSelect { get; set; }        
        public int RowCountSelect { get; set; }
        public int ColumnCount { get; set; }
        public int RowCount { get; set; }
        public int WorkSheetNumber { get; set; }
        public string UploadFileAccept { get; set; }
        public string RussianFormName { get; set; }
        public string OnCommitSuccessFunction { get; set; }
        
    }
}
