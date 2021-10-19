using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace APIConsume.Models
{
    [XmlRoot(ElementName = "Product")]
    public class Product
    {
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Description")]
        public string Description { get; set; }

        [XmlAttribute(AttributeName = "ExpirationDate")]
        public DateTime ExpirationDate { get; set; }

        [XmlAttribute(AttributeName = "DateOfProduction")]
        public DateTime DateOfProduction { get; set; }

        [XmlAttribute(AttributeName = "Quantity")]
        public int Quantity { get; set; }

        [XmlAttribute(AttributeName = "Price")]
        public float Price { get; set; }
    }
}
