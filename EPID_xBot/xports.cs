using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPID_xBot
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class xports
    {

        private xportsXporter xporterField;

        /// <remarks/>
        public xportsXporter xporter
        {
            get
            {
                return this.xporterField;
            }
            set
            {
                this.xporterField = value;
            }
        }

        public DataTable Query()
        {
            // Get the current lagDate
            //DateTime lagDate = DateTime.Today.AddDays(-Settings.lagDays);
            // Replace stuff in the SQLstring
            string sqlQuery = this.xporter.SQLstring.Replace("this.lastActive", this.xporter.lastActivity).Replace(@"\\", @"\").Trim();
            DataTable plans = SqlInterface.Query(sqlQuery);
            return plans;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class xportsXporter
    {

        private string nameField;

        private bool activeField;

        private string ipstringField;

        private byte portField;

        private string aEtitleField;

        private string sQLstringField;

        private string[] includeField;

        private string lastActivityField;

        private bool allowDoubletsField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public bool active
        {
            get
            {
                return this.activeField;
            }
            set
            {
                this.activeField = value;
            }
        }

        /// <remarks/>
        public string ipstring
        {
            get
            {
                return this.ipstringField;
            }
            set
            {
                this.ipstringField = value;
            }
        }

        /// <remarks/>
        public byte port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
            }
        }

        /// <remarks/>
        public string AEtitle
        {
            get
            {
                return this.aEtitleField;
            }
            set
            {
                this.aEtitleField = value;
            }
        }

        /// <remarks/>
        public string SQLstring
        {
            get
            {
                return this.sQLstringField;
            }
            set
            {
                this.sQLstringField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("item", IsNullable = false)]
        public string[] include
        {
            get
            {
                return this.includeField;
            }
            set
            {
                this.includeField = value;
            }
        }

        /// <remarks/>
        public string lastActivity
        {
            get
            {
                return this.lastActivityField;
            }
            set
            {
                this.lastActivityField = value;
            }
        }

        /// <remarks/>
        public bool allowDoublets
        {
            get
            {
                return this.allowDoubletsField;
            }
            set
            {
                this.allowDoubletsField = value;
            }
        }
    }

    public class JSONRoot
    {
        public xports xports { get; set; }
    }


}
