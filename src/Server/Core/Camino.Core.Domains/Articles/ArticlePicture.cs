﻿using Camino.Core.Domains.Media;

namespace Camino.Core.Domains.Articles
{
    public class ArticlePicture
    {
        public long Id { get; set; }
        public long ArticleId { get; set; }
        public long PictureId { get; set; }
        public int PictureTypeId { get; set; }
        public virtual Article Article { get; set; }
        public virtual Picture Picture { get; set; }
    }
}
