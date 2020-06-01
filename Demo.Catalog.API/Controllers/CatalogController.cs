using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Demo.Catalog.API.Extensions;
using Demo.Catalog.API.Infrastructure;
using Demo.Catalog.API.IntegrationEvents;
using Demo.Catalog.API.IntegrationEvents.Events;
using Demo.Catalog.API.Model;
using Demo.Catalog.API.ViewModel;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Demo.Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _catalogContext;
        private readonly CatalogSettings _settings;
        private readonly ICatalogIntegrationEventService _catalogIntegrationEventService;

        public CatalogController(CatalogContext context, IOptionsSnapshot<CatalogSettings> settings, ICatalogIntegrationEventService catalogIntegrationEventService)
        {
            _catalogContext = context ?? throw new ArgumentNullException(nameof(context));
            _catalogIntegrationEventService = catalogIntegrationEventService ?? throw new ArgumentNullException(nameof(catalogIntegrationEventService));
            _settings = settings.Value;

            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <summary>
        /// 分页查找商品，eg：GET api/[controller]/items[?pageSize=3&pageIndex=10]
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("items")]
        [ProducesResponseType(typeof(PagedList<CatalogItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<CatalogItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ItemsAsync([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0, [FromQuery]string ids = null)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = await GetItemsByIdsAsync(ids);

                if (!items.Any())
                {
                    return BadRequest("ids value invalid. Must be comma-separated list of numbers");
                }

                return Ok(items);
            }

            var totalItems = await _catalogContext.CatalogItems.CountAsync();

            var itemsOnPage = await _catalogContext.CatalogItems
                .OrderBy(c => c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            var model = new PagedList<CatalogItem>(itemsOnPage, totalItems, pageIndex, pageSize );

            return Ok(model);
        }
        /// <summary>
        /// 根据ID获取商品，eg：GET api/[controller]/items/7
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("items/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CatalogItem), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CatalogItem>> ItemByIdAsync(long id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var item = await _catalogContext.CatalogItems.SingleOrDefaultAsync(ci => ci.Id == id);
            var baseUri = _settings.PicBaseUrl;
            item.FillProductUrl(baseUri);
            if (item != null)
            {
                return item;
            }
            return NotFound();
        }

        /// <summary>
        /// 根据商品名分页查询商品，eg：GET api/[controller]/items/withname/samplename[?pageSize=3&pageIndex=10]
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("items/withname/{name:minlength(1)}")]
        [ProducesResponseType(typeof(PagedList<CatalogItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedList<CatalogItem>>> ItemsWithNameAsync(string name, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            var totalItems = await _catalogContext.CatalogItems
                .Where(c => c.Name.Contains(name))
                .CountAsync();
            var itemsOnPage = await _catalogContext.CatalogItems
                .Where(c => c.Name.Contains(name))
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();
            itemsOnPage = ChangeUriPlaceholder(itemsOnPage);
            return new PagedList<CatalogItem>(itemsOnPage, totalItems,pageIndex, pageSize);
        }

        /// <summary>
        /// 根据商品品牌和类型查询，eg：GET api/[controller]/items/type/1/brand[?pageSize=3&pageIndex=10]
        /// </summary>
        /// <param name="catalogTypeId"></param>
        /// <param name="catalogBrandId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("items/type/{catalogTypeId}/brand/{catalogBrandId:int?}")]
        [ProducesResponseType(typeof(PagedList<CatalogItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedList<CatalogItem>>> ItemsByTypeIdAndBrandIdAsync(int catalogTypeId, int? catalogBrandId, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            var root = (IQueryable<CatalogItem>)_catalogContext.CatalogItems;
            root = root.Where(ci => ci.CatalogTypeId == catalogTypeId);

            if (catalogBrandId.HasValue)
            {
                root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
            }

            var totalItems = await root.CountAsync();

            var itemsOnPage = await root
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            return new PagedList<CatalogItem>(itemsOnPage, totalItems, pageIndex, pageSize);
        }

        /// <summary>
        /// 根据品牌查询，GET api/[controller]/items/type/all/brand[?pageSize=3&pageIndex=10]
        /// </summary>
        /// <param name="catalogBrandId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("items/type/all/brand/{catalogBrandId:int?}")]
        [ProducesResponseType(typeof(PagedList<CatalogItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedList<CatalogItem>>> ItemsByBrandIdAsync(int? catalogBrandId, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            var root = (IQueryable<CatalogItem>)_catalogContext.CatalogItems;

            if (catalogBrandId.HasValue)
            {
                root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
            }

            var totalItems = await root.CountAsync();

            var itemsOnPage = await root
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            return new PagedList<CatalogItem>(itemsOnPage, totalItems, pageIndex, pageSize);
        }

        /// <summary>
        /// 查询所有商品类型，eg：GET api/[controller]/CatalogTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("catalogtypes")]
        [ProducesResponseType(typeof(List<CatalogType>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<CatalogType>>> CatalogTypesAsync()
        {
            return await _catalogContext.CatalogTypes.ToListAsync();
        }
        /// <summary>
        /// 查询所有商品类型，eg：GET api/[controller]/CatalogBrands
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("catalogbrands")]
        [ProducesResponseType(typeof(List<CatalogBrand>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<CatalogBrand>>> CatalogBrandsAsync()
        {
            return await _catalogContext.CatalogBrands.ToListAsync();
        }
        /// <summary>
        /// 更新商品信息，eg：PUT api/[controller]/items
        /// </summary>
        /// <param name="productToUpdate"></param>
        /// <returns></returns>
        [Route("items")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> UpdateProductAsync([FromBody]CatalogItem productToUpdate, [FromServices] ICapPublisher capBus)
        {
            var catalogItem = await _catalogContext.CatalogItems.SingleOrDefaultAsync(i => i.Id == productToUpdate.Id);

            if (catalogItem == null)
            {
                return NotFound(new { Message = $"Item with id {productToUpdate.Id} not found." });
            }

            var oldPrice = catalogItem.Price;
            var raiseProductPriceChangedEvent = oldPrice != productToUpdate.Price;

            catalogItem = productToUpdate;
            
            // Save product's data and publish integration event through the Event Bus if price has changed
            if (raiseProductPriceChangedEvent) 
            {
                var priceChangedEvent = new ProductPriceChangedIntegrationEvent(catalogItem.Id, productToUpdate.Price, oldPrice); 
                var curTransaction = _catalogContext.Database.BeginTransaction(capBus);
                try
                {
                    _catalogContext.CatalogItems.Update(catalogItem);
                    //publish integration event
                    capBus.Publish(nameof(ProductPriceChangedIntegrationEvent), priceChangedEvent);
                    await _catalogContext.SaveChangesAsync();
                    curTransaction.Commit();
                }
                catch
                {
                    try
                    {
                        curTransaction?.Rollback();
                    }
                    finally
                    {
                        curTransaction?.Dispose();
                    }
                    throw;
                }
                finally
                {
                    curTransaction?.Dispose();
                }
            }
            else // Just save the updated product because the Product's Price hasn't changed.
            {
                _catalogContext.CatalogItems.Update(catalogItem);
                await _catalogContext.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(ItemByIdAsync), new { id = productToUpdate.Id }, null);
        }

        /// <summary>
        /// 创建新的商品，eg：POST api/[controller]/items
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Route("items")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> CreateProductAsync([FromBody]CatalogItem product)
        {
            var item = new CatalogItem
            {
                CatalogBrandId = product.CatalogBrandId,
                CatalogTypeId = product.CatalogTypeId,
                Description = product.Description,
                Name = product.Name,
                PictureFileName = product.PictureFileName,
                Price = product.Price
            };

            _catalogContext.CatalogItems.Add(item);

            await _catalogContext.SaveChangesAsync();

            return CreatedAtAction(nameof(ItemByIdAsync), new { id = item.Id }, null);
        }

        /// <summary>
        /// 删除商品项，eg：DELETE api/[controller]/id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> DeleteProductAsync(int id)
        {
            var product = _catalogContext.CatalogItems.SingleOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            _catalogContext.CatalogItems.Remove(product);
            await _catalogContext.SaveChangesAsync();

            return NoContent();
        }

        #region Private Methods
        /// <summary>
        /// 将id串转成id集合，查询所有商品
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private async Task<List<CatalogItem>> GetItemsByIdsAsync(string ids)
        {
            var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));
            if (!numIds.All(nid => nid.Ok))
            {
                return new List<CatalogItem>();
            }

            var idsToSelect = numIds.Select(id => id.Value);

            var items = await _catalogContext.CatalogItems.Where(ci => idsToSelect.Contains(ci.Id)).ToListAsync();

            items = ChangeUriPlaceholder(items);

            return items;
        }

        private List<CatalogItem> ChangeUriPlaceholder(List<CatalogItem> items)
        {
            var baseUri = _settings.PicBaseUrl;

            foreach (var item in items)
            {
                item.FillProductUrl(baseUri);
            }

            return items;
        }
        #endregion
    }
}
