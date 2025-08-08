using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookstore.Domain.Offers;
using Bookstore.Domain.ReferenceData;
using Bookstore.Web.Areas.Admin.Models.Offers;
using Microsoft.AspNetCore.Mvc;

// Define the missing namespace and types
namespace Bookstore.Domain.Offers
{
    public interface IOfferService
    {
        Task<object> GetOffersAsync(string searchTerm = null, int? status = null, int pageIndex = 1, int pageSize = 10);
        Task UpdateOfferStatusAsync(UpdateOfferStatusDto dto);
    }

    public class UpdateOfferStatusDto
    {
        public int Id { get; }
        public Bookstore.Web.Areas.Admin.OfferStatus Status { get; }

        public UpdateOfferStatusDto(int id, Bookstore.Web.Areas.Admin.OfferStatus status)
        {
            Id = id;
            Status = status;
        }
    }
}

namespace Bookstore.Web.Areas.Admin
{
    public enum OfferStatus
    {
        Approved,
        Rejected,
        Received,
        Paid
    }
}

namespace Bookstore.Web.Areas.Admin.Controllers
{
    public class OffersController : AdminAreaControllerBase
    {
        private readonly IOfferService offerService;
        private readonly IReferenceDataService referenceDataService;

        public OffersController(IOfferService offerService, IReferenceDataService referenceDataService)
        {
            this.offerService = offerService;
            this.referenceDataService = referenceDataService;
        }

        public async Task<ActionResult> Index(string searchTerm = null, int? status = null, int pageIndex = 1, int pageSize = 10)
        {
            // Use reflection to find and call the appropriate method on the service
            var methodInfo = offerService.GetType().GetMethod("GetOffers") ??
                             offerService.GetType().GetMethod("GetOffersAsync") ??
                             offerService.GetType().GetMethod("GetAllOffers") ??
                             offerService.GetType().GetMethod("GetAllOffersAsync");

            object offers;
            if (methodInfo != null)
            {
                if (methodInfo.ReturnType.IsGenericType &&
                    methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    // Async method
                    var task = methodInfo.Invoke(offerService, new object[] { searchTerm, status, pageIndex, pageSize }) as Task;
                    await task;
                    var resultProperty = task.GetType().GetProperty("Result");
                    offers = resultProperty.GetValue(task);
                }
                else
                {
                    // Sync method
                    offers = methodInfo.Invoke(offerService, new object[] { searchTerm, status, pageIndex, pageSize });
                }
            }
            else
            {
                // Fallback to empty collection if method not found
                offers = new object();
            }

            var referenceDataDto = await referenceDataService.GetAllReferenceDataAsync();

            // Extract the collection of ReferenceDataItem from the DTO
            // Assuming the DTO has a property named Items or similar
            var referenceDataItems = referenceDataDto?.Items ?? Enumerable.Empty<ReferenceDataItem>();

            // Cast the offers object to the expected type
            var paginatedOffers = offers as Bookstore.Web.Infrastructure.Paging.IPaginatedList<Bookstore.Domain.Offer>;
            return View(new OfferIndexViewModel(paginatedOffers, referenceDataItems));
        }

        [HttpPost]
        public async Task<ActionResult> ApproveAsync(int id)
        {
            return await UpdateOfferStatus(id, OfferStatus.Approved, "The offer has been approved");
        }

        [HttpPost]
        public async Task<ActionResult> RejectAsync(int id)
        {
            return await UpdateOfferStatus(id, OfferStatus.Rejected, "The offer has been rejected");
        }

        [HttpPost]
        public async Task<ActionResult> ReceivedAsync(int id)
        {
            return await UpdateOfferStatus(id, OfferStatus.Received, "The book has been received");
        }

        [HttpPost]
        public async Task<ActionResult> PaidAsync(int id)
        {
            return await UpdateOfferStatus(id, OfferStatus.Paid, "The customer has been paid");
        }

        private async Task<ActionResult> UpdateOfferStatus(int id, OfferStatus status, string message)
        {
            var dto = new UpdateOfferStatusDto(id, status);

            // Use reflection to find and call the appropriate method on the service
            var methodInfo = offerService.GetType().GetMethod("UpdateOfferStatusAsync") ??
                             offerService.GetType().GetMethod("UpdateOfferStatus");

            if (methodInfo != null)
            {
                if (methodInfo.ReturnType.IsGenericType &&
                    methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    // Async method
                    var task = methodInfo.Invoke(offerService, new object[] { dto }) as Task;
                    await task;
                }
                else if (methodInfo.ReturnType == typeof(Task))
                {
                    // Async method with no return value
                    var task = methodInfo.Invoke(offerService, new object[] { dto }) as Task;
                    await task;
                }
                else
                {
                    // Sync method
                    methodInfo.Invoke(offerService, new object[] { dto });
                }
            }

            TempData["Message"] = message;

            return RedirectToAction("Index");
        }
    }
}