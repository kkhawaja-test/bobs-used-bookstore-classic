using Bookstore.Domain.ReferenceData;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace Bookstore.Web.ViewModel.Resale
{
    public class ResaleCreateViewModel
    {
        public ResaleCreateViewModel() { }

        public ResaleCreateViewModel(IEnumerable<ReferenceDataItem> referenceDataItems)
        {
            var bookTypeItems = referenceDataItems.Where(x => x.GetType().Name.Contains("BookType") ||
                (x.GetType().GetProperty("Category")?.GetValue(x)?.ToString() == "BookType")).ToList();
            var publisherItems = referenceDataItems.Where(x => x.GetType().Name.Contains("Publisher") ||
                (x.GetType().GetProperty("Category")?.GetValue(x)?.ToString() == "Publisher")).ToList();
            var genreItems = referenceDataItems.Where(x => x.GetType().Name.Contains("Genre") ||
                (x.GetType().GetProperty("Category")?.GetValue(x)?.ToString() == "Genre")).ToList();
            var conditionItems = referenceDataItems.Where(x => x.GetType().Name.Contains("Condition") ||
                (x.GetType().GetProperty("Category")?.GetValue(x)?.ToString() == "Condition")).ToList();

            BookTypes = bookTypeItems.Select(x => new SelectListItem { Value = x.GetType().GetProperty("Id")?.GetValue(x)?.ToString(), Text = x.GetType().GetProperty("Name")?.GetValue(x)?.ToString() });
            Publishers = publisherItems.Select(x => new SelectListItem { Value = x.GetType().GetProperty("Id")?.GetValue(x)?.ToString(), Text = x.GetType().GetProperty("Name")?.GetValue(x)?.ToString() });
            Genres = genreItems.Select(x => new SelectListItem { Value = x.GetType().GetProperty("Id")?.GetValue(x)?.ToString(), Text = x.GetType().GetProperty("Name")?.GetValue(x)?.ToString() });
            Conditions = conditionItems.Select(x => new SelectListItem { Value = x.GetType().GetProperty("Id")?.GetValue(x)?.ToString(), Text = x.GetType().GetProperty("Name")?.GetValue(x)?.ToString() });
        }

        public IEnumerable<SelectListItem> BookTypes { get; internal set; }

        public IEnumerable<SelectListItem> Publishers { get; internal set; }

        public IEnumerable<SelectListItem> Genres { get; internal set; }

        public IEnumerable<SelectListItem> Conditions { get; internal set; }

        public int SelectedBookTypeId { get; set; }

        public int SelectedPublisherId { get; set; }

        public int SelectedGenreId { get; set; }

        public int SelectedConditionId { get; set; }

        public decimal BookPrice { get; set; }

        public string BookName { get; set; }

        public string Author { get; set; }

        public string ISBN { get; set; }
    }
}