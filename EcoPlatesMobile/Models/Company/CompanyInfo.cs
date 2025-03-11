using EcoPlatesMobile.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Company
{
    public class CompanyInfo
    {
        public long company_id { get; set; }                          
        public string company_name { get; set; }                     
        public string phone_number { get; set; }                    
        public string? logo_url { get; set; }                         
        public int rating { get; set; }                               
        public double? location_latitude { get; set; }                
        public double? location_longitude { get; set; }               
        public double? distance_km { get; set; }                      
        public string? working_hours { get; set; }                    
        public string? telegram_link { get; set; }                    
        public string? social_profile_link { get; set; }              
        public UserOrCompanyStatus status { get; set; }              
        public bool deleted { get; set; }                             
    }
}
