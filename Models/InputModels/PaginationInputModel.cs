using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PWANews.InputModels
{
    public class PaginationInputModel
    {
        [Range(0,100)]
        public int Offset { get; set; } = 0;

        [Range(1,50)]
        public int Size { get; set; } = 50;

    }
}
