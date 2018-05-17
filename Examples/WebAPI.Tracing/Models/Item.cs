using JetBrains.Annotations;

namespace WebApiService.Tracing.Models
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }
}