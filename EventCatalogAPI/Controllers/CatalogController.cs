using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventCatalogAPI.Data;
using EventCatalogAPI.Domain;
using EventCatalogAPI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EventCatalogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _context;
        private readonly IConfiguration _config;
        public CatalogController(CatalogContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> Items(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 6)
        {
            var itemsCount = _context.EventItems.LongCountAsync();
            var items = await _context.EventItems
                .OrderBy(c => c.Name)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            items = ChangePictureUrl(items);

            var model = new PaginatedItemsViewModel<EventItem>
            {
                PageIndex = pageIndex,
                pageSize = pageSize,
                Count = itemsCount.Result,
                Data = items
            };

            return Ok(model);
        }

        private List<EventItem> ChangePictureUrl(List<EventItem> items)
        {
            items.ForEach(item =>
            item.PictureUrl = item.PictureUrl.Replace(
                                "http://externalcatalogbaseurltobereplaced",
                                _config["ExternalCatalogBaseUrl"]));

            return items;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> EventFormats()
        {
            var formats = await _context.EventFormats.ToListAsync();
            return Ok(formats);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> EventCategories()
        {
            var categories = await _context.EventCategories.ToListAsync();
            return Ok(categories);
        }

        [HttpGet("[action]/format/{eventFormatId}/category/{eventCategoryId}")]
        public async Task<IActionResult> Items(
            int? eventFormatId,
            int? eventCategoryId,
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 6
            )
        {
            var query = (IQueryable<EventItem>)_context.EventItems
                    .Include(e => e.EventCategory)
                    .Include(e => e.EventFormat);

            if (eventFormatId.HasValue)
            {
                query = query.Where(e => e.EventFormatId == eventFormatId);
            }
            if (eventCategoryId.HasValue)
            {
                query = query.Where(e => e.EventCategoryId == eventCategoryId);
            }

            var itemsCount = query.LongCountAsync();
            var items = await query
                .OrderBy(c => c.Name)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            items = ChangePictureUrl(items);

            var model = new PaginatedItemsViewModel<EventItem>
            {
                PageIndex = pageIndex,
                pageSize = pageSize,
                Count = itemsCount.Result,
                Data = items
            };

            return Ok(model);
        }
    }
}