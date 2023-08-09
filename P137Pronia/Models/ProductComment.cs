namespace P137Pronia.Models;

public class ProductComment:BaseEntity
{
    public string AppUserId { get; set; }
    public int ProductId { get; set; }
    public DateTime PostedTime { get; set; }
    public AppUser AppUser { get; set; }
    public Product Product { get; set; }
    public  bool IsDeleted { get; set; }


}
