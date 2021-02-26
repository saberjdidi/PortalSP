using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class Request
    {
        public int id { get; set; }
        public string code { get; set; }
        //public DateTime? creationDate { get; set; }
        public int ambulatoryRequest { get; set; }
        public string creationDate { get; set; }
        public string executionDate { get; set; }
        public string checkDate { get; set; }
        public string samplingDate { get; set; }
        public string groupId { get; set; }
        public Branch branch { get; set; }
        public Patient patient { get; set; }
        public Nomenclatura nomenclatura { get; set; }
        public Client client { get; set; }
        public Status status { get; set; }
        //public string status { get; set; }
        public Icdo icdo { get; set; }
        public Siapec siapec { get; set; }
        public bool billDownloaded { get; set; }
        public Double price { get; set; }
        public Instrument instrument { get; set; }
        public Room room { get; set; }
        public Requestcatalog requestCatalog { get; set; }
        public List<BiologicalMaterials> biologicalMaterials { get; set; }
        public bool isCloned { get; set; }
        public bool isHandled { get; set; }
        public bool isMaster { get; set; }
        public bool isPositive { get; set; }
        public User createBy { get; set; }
        public string doctorAvisChls { get; set; }
        public DoctorNoRef doctorNoRef { get; set; }
        public string drugDescription { get; set; }
    }
}
