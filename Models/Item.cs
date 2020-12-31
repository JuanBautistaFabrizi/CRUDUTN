using System;

namespace Infraestructura.Models
{

public class Item
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string State { get; set; }

    public bool IsComplete {get; set; }

    public User Responsible {get; set; }

    public string Photo { get; set; }

    public DateTime Date { get; set; }

        
    }
}
    
