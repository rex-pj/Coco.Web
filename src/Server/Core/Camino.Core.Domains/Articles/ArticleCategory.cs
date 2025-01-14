﻿namespace Camino.Core.Domains.Articles
{
    public class ArticleCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public DateTime UpdatedDate { get; set; }
        
        public long UpdatedById { get; set; }
        
        public DateTime CreatedDate { get; set; }

        public long CreatedById { get; set; }

        public int? ParentId { get; set; }
        public int StatusId { get; set; }

        public virtual ArticleCategory ParentCategory { get; set; }

        public virtual ICollection<ArticleCategory> ChildCategories { get; set; }
        public virtual ICollection<Article> Articles { get; set; }
    }
}
