using System.Collections.Generic;

namespace Infraestructura.Models
{
    public class ItemList
    {
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public List<Item> Items { get; set; }
    }
}