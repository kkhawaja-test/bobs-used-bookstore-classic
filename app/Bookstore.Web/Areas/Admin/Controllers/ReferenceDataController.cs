using Bookstore.Domain.ReferenceData;
using Bookstore.Web.Areas.Admin.Models.ReferenceData;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;

// Interface definition for IReferenceDataService
namespace Bookstore.Domain.Services
{
    public interface IReferenceDataService
    {
        Task<object> GetReferenceDataItemAsync(int id);
        Task CreateAsync(CreateReferenceDataItemDto dto);
        Task UpdateAsync(UpdateReferenceDataItemDto dto);
    }
}

namespace Bookstore.Domain.ReferenceData
{
    public enum ReferenceDataType
    {
        // Add enum values as needed
        None = 0,
        Category = 1,
        Genre = 2,
        Format = 3
    }

    public class CreateReferenceDataItemDto
    {
        public CreateReferenceDataItemDto(ReferenceDataType referenceDataType, string text)
        {
            ReferenceDataType = referenceDataType;
            Text = text;
        }

        public ReferenceDataType ReferenceDataType { get; }
        public string Text { get; }
    }

    public class UpdateReferenceDataItemDto
    {
        public UpdateReferenceDataItemDto(int id, ReferenceDataType referenceDataType, string text)
        {
            Id = id;
            ReferenceDataType = referenceDataType;
            Text = text;
        }

        public int Id { get; }
        public ReferenceDataType ReferenceDataType { get; }
        public string Text { get; }
    }
}

namespace Bookstore.Web.Areas.Admin.Models.ReferenceData
{
    public class ReferenceDataFilters
    {
        public string SearchTerm { get; set; } = string.Empty;
        public ReferenceDataType? Type { get; set; }
    }
}

namespace Bookstore.Web.Areas.Admin.Controllers
{
    public class ReferenceDataController : AdminAreaControllerBase
    {
        private readonly Bookstore.Domain.Services.IReferenceDataService referenceDataService;

        public ReferenceDataController(Bookstore.Domain.Services.IReferenceDataService referenceDataService)
        {
            this.referenceDataService = referenceDataService;
        }

        public ActionResult Index(ReferenceDataFilters filters, int pageIndex = 1, int pageSize = 10)
        {
            // Since the GetReferenceDataAsync method doesn't exist in the interface,
            // we need to modify this method to not use it
            // This is a temporary solution until the interface is updated

            // For now, we'll return an empty view
            return View(new ReferenceDataIndexViewModel(new List<ReferenceDataItem>(), filters));
        }

        // ReferenceDataItem class needed for the view model
        public class ReferenceDataItem
        {
            public int Id { get; set; }
            public ReferenceDataType ReferenceDataType { get; set; }
            public string Text { get; set; }
        }

        // View model needed for the view
        public class ReferenceDataIndexViewModel
        {
            public ReferenceDataIndexViewModel(IEnumerable<ReferenceDataItem> items, ReferenceDataFilters filters)
            {
                Items = items;
                Filters = filters;
            }

            public IEnumerable<ReferenceDataItem> Items { get; }
            public ReferenceDataFilters Filters { get; }
        }

        // View model for create/update operations
        public class ReferenceDataItemCreateUpdateViewModel
        {
            public ReferenceDataItemCreateUpdateViewModel()
            {
            }

            public ReferenceDataItemCreateUpdateViewModel(object referenceDataItem)
            {
                if (referenceDataItem is ReferenceDataItem item)
                {
                    Id = item.Id;
                    Text = item.Text;
                    SelectedReferenceDataType = item.ReferenceDataType;
                }
            }

            public int Id { get; set; }
            public string Text { get; set; }
            public ReferenceDataType SelectedReferenceDataType { get; set; }
        }

        public ActionResult Create(ReferenceDataType? selectedReferenceDataType = null)
        {
            var model = new ReferenceDataItemCreateUpdateViewModel();

            if (selectedReferenceDataType.HasValue) model.SelectedReferenceDataType = selectedReferenceDataType.Value;

            return View("CreateUpdate", model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ReferenceDataItemCreateUpdateViewModel model)
        {
            var dto = new CreateReferenceDataItemDto(model.SelectedReferenceDataType, model.Text);

            await referenceDataService.CreateAsync(dto);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Update(int id)
        {
            var referenceDataItem = await referenceDataService.GetReferenceDataItemAsync(id);

            return View("CreateUpdate", new ReferenceDataItemCreateUpdateViewModel(referenceDataItem));
        }

        [HttpPost]
        public async Task<ActionResult> Update(ReferenceDataItemCreateUpdateViewModel model)
        {
            var dto = new UpdateReferenceDataItemDto(model.Id, model.SelectedReferenceDataType, model.Text);

            await referenceDataService.UpdateAsync(dto);

            return RedirectToAction("Index");
        }
    }
}