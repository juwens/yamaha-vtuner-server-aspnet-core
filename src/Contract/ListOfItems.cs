using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VtnrNetRadioServer.Contract
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class ListOfItems
    {

        private byte itemCountField;

        private ListOfItemsItem[] itemField;

        /// <remarks/>
        public byte ItemCount
        {
            get
            {
                return this.itemCountField;
            }
            set
            {
                this.itemCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Item")]
        public ListOfItemsItem[] Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ListOfItemsItem
    {

        private string itemTypeField;

        private ushort stationIdField;

        private bool stationIdFieldSpecified;

        private string stationNameField;

        private string stationUrlField;

        private string stationDescField;

        private string logoField;

        private string stationFormatField;

        private string stationLocationField;

        private byte stationBandWidthField;

        private bool stationBandWidthFieldSpecified;

        private string stationMimeField;

        private byte reliaField;

        private bool reliaFieldSpecified;

        private string bookmarkField;

        private string urlPreviousField;

        private string urlPreviousBackUpField;

        /// <remarks/>
        public string ItemType
        {
            get
            {
                return this.itemTypeField;
            }
            set
            {
                this.itemTypeField = value;
            }
        }

        /// <remarks/>
        public ushort StationId
        {
            get
            {
                return this.stationIdField;
            }
            set
            {
                this.stationIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StationIdSpecified
        {
            get
            {
                return this.stationIdFieldSpecified;
            }
            set
            {
                this.stationIdFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string StationName
        {
            get
            {
                return this.stationNameField;
            }
            set
            {
                this.stationNameField = value;
            }
        }

        /// <remarks/>
        public string StationUrl
        {
            get
            {
                return this.stationUrlField;
            }
            set
            {
                this.stationUrlField = value;
            }
        }

        /// <remarks/>
        public string StationDesc
        {
            get
            {
                return this.stationDescField;
            }
            set
            {
                this.stationDescField = value;
            }
        }

        /// <remarks/>
        public string Logo
        {
            get
            {
                return this.logoField;
            }
            set
            {
                this.logoField = value;
            }
        }

        /// <remarks/>
        public string StationFormat
        {
            get
            {
                return this.stationFormatField;
            }
            set
            {
                this.stationFormatField = value;
            }
        }

        /// <remarks/>
        public string StationLocation
        {
            get
            {
                return this.stationLocationField;
            }
            set
            {
                this.stationLocationField = value;
            }
        }

        /// <remarks/>
        public byte StationBandWidth
        {
            get
            {
                return this.stationBandWidthField;
            }
            set
            {
                this.stationBandWidthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StationBandWidthSpecified
        {
            get
            {
                return this.stationBandWidthFieldSpecified;
            }
            set
            {
                this.stationBandWidthFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string StationMime
        {
            get
            {
                return this.stationMimeField;
            }
            set
            {
                this.stationMimeField = value;
            }
        }

        /// <remarks/>
        public byte Relia
        {
            get
            {
                return this.reliaField;
            }
            set
            {
                this.reliaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ReliaSpecified
        {
            get
            {
                return this.reliaFieldSpecified;
            }
            set
            {
                this.reliaFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string Bookmark
        {
            get
            {
                return this.bookmarkField;
            }
            set
            {
                this.bookmarkField = value;
            }
        }

        /// <remarks/>
        public string UrlPrevious
        {
            get
            {
                return this.urlPreviousField;
            }
            set
            {
                this.urlPreviousField = value;
            }
        }

        /// <remarks/>
        public string UrlPreviousBackUp
        {
            get
            {
                return this.urlPreviousBackUpField;
            }
            set
            {
                this.urlPreviousBackUpField = value;
            }
        }
    }


}
