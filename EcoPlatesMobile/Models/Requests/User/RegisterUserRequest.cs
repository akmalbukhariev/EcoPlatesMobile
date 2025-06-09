using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Requests.User
{
    public class RegisterUserRequest
    {
        public string? phone_number { get; set; }    
        public string? first_name { get; set; }
        public double? location_latitude { get; set; }       
        public double? location_longitude { get; set; }      
    }
}
