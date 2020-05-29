using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Catalog.API.Model
{
    public class CatalogBrand
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Brand { get; set; }
    }
}
