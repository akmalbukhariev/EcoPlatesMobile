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
        public string? email { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? full_name { get; set; }
        public double? location_latitude { get; set; }       
        public double? location_longitude { get; set; }      
        public string? profile_picture_url { get; set; }
        public Stream? profile_picture_data { get; set; }
    }
}
