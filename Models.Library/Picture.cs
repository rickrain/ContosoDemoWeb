using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Models.Library
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class Pictures : DbContext
    {
        public Pictures()
            : base(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString)
        {
        }

        public virtual DbSet<Picture> PictureEntities { get; set; }
    }

    public class Picture
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
    }

}
