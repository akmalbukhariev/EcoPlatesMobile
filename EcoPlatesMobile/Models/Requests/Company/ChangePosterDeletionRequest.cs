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
        private string poster_id { get; set; }
        private bool deleted { get; set; }
    }
}
