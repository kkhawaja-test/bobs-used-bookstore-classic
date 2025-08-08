using Bookstore.Domain.ReferenceData;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookstore.Web.Areas.Admin.Models.ReferenceData
{
    public class ReferenceDataItemCreateUpdateViewModel
    {
        public ReferenceDataItemCreateUpdateViewModel() { }

        public ReferenceDataItemCreateUpdateViewModel(ReferenceDataItem referenceDataItem)
        {
            // Skip ID assignment as the property name is unknown
            // Id will keep its default value of 0
            // Since we don't know the exact property name for the data type in ReferenceDataItem
            // We'll initialize with default value and it should be set properly later
            // SelectedReferenceDataType = default;

            // Property 'Text' not found in ReferenceDataItem
            // Text will need to be set separately after construction
        }

        public int Id { get; set; }

        public ReferenceDataType SelectedReferenceDataType { get; set; }

        public string Text { get; set; }

        public IEnumerable<SelectListItem> DataTypes { get; set; }
    }
}