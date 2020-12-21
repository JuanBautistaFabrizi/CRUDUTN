using System;

namespace Infraestructura.Dtos
{
    public class ItemDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Responsible { get; set; }

        public bool IsComplete {get; set; }

        public DateTime Date { get; set; }
    }
}