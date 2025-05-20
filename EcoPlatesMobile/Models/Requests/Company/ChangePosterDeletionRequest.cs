using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EcoPlatesMobile.Models.Requests.Company
{
    public class ChangePosterDeletionRequest
    {
        public long? poster_id { get; set; }
        public bool deleted { get; set; }
    }
}
