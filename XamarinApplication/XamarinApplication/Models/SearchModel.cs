using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class SearchModel
    {
        /*public long id1 = -1;
        public long id2 = -1;
        public long id3 = -1;
        public long nomenclatureId = -1;
        public string criteria0 = "";
        public string criteria1 = "";
        public string criteria2 = "";
        public string criteria3 = "";
        public string criteria4 = "";
        public string criteria5 = "";
        public string status = "";
        public DateTime? date = null;
        public DateTime? date1 = null;
        public long ambulatoireService = -1;
        public string check1 = null; 
        public string positive = null;
        public int downloadStatus = 0;
        public int offset = 0;
        public int maxResult;
        public string order;
        public string sortedBy;*/


        #region Declaration
        private int s_id1;
        private int s_id2;
        private int s_id3;
        private long s_nomenclatureId;
        private string s_criteria0;
        private string s_criteria1;
        private string s_criteria2;
        private string s_criteria3;
        private string s_criteria4;
        private string s_criteria5;
        private string s_status;
        private string s_date2;
        private DateTime s_date;
        private DateTime s_date1;
        private long s_ambulatoireService;
        private bool s_check1;
        private string s_positive;
        private int s_downloadStatus;
        private int s_offset;
        private int s_maxResult;
        private string s_order;
        private string s_sortedBy;

        #endregion

        #region Property
        public int id1 
        {
            get { return s_id1; }
            set { this.s_id1 = value; }
        }
        public int id2
        {
            get { return s_id2; }
            set { this.s_id2 = value; }
        }
        public int id3
        {
            get { return s_id3; }
            set { this.s_id3 = value; }
        }
        public long nomenclatureId
        {
            get { return s_nomenclatureId; }
            set { this.s_nomenclatureId = value; }
        }
        public string criteria0
        {
            get { return s_criteria0; }
            set { this.s_criteria0 = value; }
        }
        public string criteria1
        {
            get { return s_criteria1; }
            set { this.s_criteria1 = value; }
        }
        public string criteria2
        {
            get { return s_criteria2; }
            set { this.s_criteria2 = value; }
        }
        public string criteria3
        {
            get { return s_criteria3; }
            set { this.s_criteria3 = value; }
        }
        public string criteria4
        {
            get { return s_criteria4; }
            set { this.s_criteria4 = value; }
        }
        public string criteria5
        {
            get { return s_criteria5; }
            set { this.s_criteria5 = value; }
        }
        public string status
        {
            get { return s_status; }
            set { this.s_status = value; }
        }
        public string date2
        {
            get { return s_date2; }
            set { this.s_date2 = value; }
        }
        public DateTime date
        {
            get { return s_date; }
            set { this.s_date = value; }
        }
        public DateTime date1
        {
            get { return s_date1; }
            set { this.s_date1 = value; }
        }
        public long ambulatoireService
        {
            get { return s_ambulatoireService; }
            set { this.s_ambulatoireService = value; }
        }
        public bool check1
        {
            get { return s_check1; }
            set { this.s_check1 = value; }
        }
        public string positive
        {
            get { return s_positive; }
            set { this.s_positive = value; }
        }
        public int downloadStatus
        {
            get { return s_downloadStatus; }
            set { this.s_downloadStatus = value; }
        }
        public int offset
        {
            get { return s_offset; }
            set { this.s_offset = value; }
        }
        public int maxResult
        {
            get { return s_maxResult; }
            set { this.s_maxResult = value; }
        }
        public string order
        {
            get { return s_order; }
            set { this.s_order = value; }
        }
        public string sortedBy
        {
            get { return s_sortedBy; }
            set { this.s_sortedBy = value; }
        }
        public bool fromReflex { get; set; }
        #endregion

        #region Constructor
        public SearchModel()
        {

            s_id1 = -1;
            s_id2 = -1;
            s_id3 = -1;
            s_nomenclatureId = -1;
            s_criteria0 = string.Empty;
            s_criteria1 = string.Empty;
            s_criteria2 = string.Empty;
            s_criteria3 = string.Empty;
            s_criteria4 = string.Empty;
            s_criteria5 = string.Empty;
            s_date2 = string.Empty;
            //s_status = string.Empty;
            s_date = default(DateTime);
            s_date1 = default(DateTime);
            s_ambulatoireService = -1;
           // s_check1 = null;
            s_positive = null;
            s_downloadStatus = 0;
            s_offset = 0;
    }
    #endregion
}
}
