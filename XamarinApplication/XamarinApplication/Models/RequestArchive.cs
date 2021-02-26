using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class RequestArchive
    {
        public int id { get; set; }
        public string barCode { get; set; }
        public bool isActive { get; set; }
        public Slot slot { get; set; }
    }
    public class Slot
    {
        public int id { get; set; }
        public int sequence { get; set; }
        public string note { get; set; }
        public string barCode { get; set; }
        public bool isActive { get; set; }
        public bool isEmpty { get; set; }
        public Drawer drawer { get; set; }
    }
    public class Drawer
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string barCode { get; set; }
        public bool isActive { get; set; }
        public Shelf shelf { get; set; }
        public string type { get; set; }
    }
    public class Shelf
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string barCode { get; set; }
        public bool isActive { get; set; }
        public bool isDefault { get; set; }
        public ShelvingUnit shelvingUnit { get; set; }
    }
    public class ShelvingUnit
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool isActive { get; set; }
        public bool isDefault { get; set; }
        public Headquarter headquarter { get; set; }
    }
    public class Headquarter
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool isActive { get; set; }
        public Company company { get; set; }
    }
    public class Company
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string storeCode { get; set; }
        public string contact { get; set; }
    }
}
