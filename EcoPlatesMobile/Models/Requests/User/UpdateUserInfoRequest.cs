using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Requests.User
{
    public class UpdateUserInfoRequest
    {
        private string? phone_number { get; set; }
        private string? email { get; set; }
        private string? first_name { get; set; }
        private string? last_name { get; set; }
        private string? full_name { get; set; }
        private double location_latitude { get; set; }
        private double location_longitude { get; set; }
        private string? profile_picture_url { get; set; }
        private Stream? profile_picture_data { get; set; }
    }
}
