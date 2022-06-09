﻿using Camino.Core.Domains.Media;

namespace Camino.Core.Domains.Products
{
    public class ProductPicture
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long PictureId { get; set; }
        public int PictureTypeId { get; set; }
        public virtual Product Product { get; set; }
        public virtual Picture Picture { get; set; }
    }
}
