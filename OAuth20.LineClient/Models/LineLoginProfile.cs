using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth20.LineClient.Models
{
    public class LineLoginProfile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string AccessToken { get; set; }
    }
}
