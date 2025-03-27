using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionManager.Core.Models
{
    public class Collection
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Hashes { get; set; } = [];
    }
}
