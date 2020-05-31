using Demo.Catalog.API.Model;

namespace Demo.Catalog.API.Extensions
{
    public static class CatalogItemExtensions
    {
        public static void FillProductUrl(this CatalogItem item, string picBaseUrl)
        {
            if (item != null)
            {
                item.PictureUri = picBaseUrl.Replace("[0]", item.Id.ToString());
            }
        }
    }
}
