﻿using MartyrGraveManagement_BAL.ModelViews.BlogDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.BlogCategoryDTOs
{
    public class BlogCategoryDtoResponse
    {
        public int HistoryId { get; set; }
        public string? BlogCategoryName { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Status { get; set; }
        public List<BlogDtoResponse>? Blogs { get; set; }
    }
}