﻿using Entities;

namespace Bussiness.Dto
{
    public class ProductCategoryForViewDto : EntityDto
    {
        public string? Name { get; set; }
        public int? ParentId { get; set; }
}
}
