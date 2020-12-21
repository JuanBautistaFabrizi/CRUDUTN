namespace Infraestructura.Dtos
{
    public class NewItemDTO
    {
          public string Name { get; set; }
          public string UserId { get; set; }
          public long Date {get; set; }
          public int State { get; set; }

          public bool IsComplete { get; set; }
    }
}