namespace ReservaSalones.Application.Models;

public class PaginatedList<T>
{
    public int? Count { get; set; }

    public List<T> Items { get; set; } = new();
}