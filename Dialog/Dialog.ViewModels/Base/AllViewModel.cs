﻿using System.Collections.Generic;
using Dialog.Common;

namespace Dialog.ViewModels.Base
{
    public class AllViewModel<T>
    {
        public int PageSize { get; set; } = 3;

        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; }

        public bool HasPreviousPage => this.CurrentPage > 1;

        public bool HasNextPage => this.CurrentPage < this.TotalPages;

        public string Author { get; set; }

        public ICollection<T> Entities { get; set; }
    }
}