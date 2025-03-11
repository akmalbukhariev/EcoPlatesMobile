using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Requests.Company
{
    public class PostersRegisterRequest
    {
        public required long company_id { get; set; }        
        public string? title { get; set; }                   
        public string? description { get; set; }             
        public required decimal old_price { get; set; }      
        public required decimal new_price { get; set; }      
        public string? image_url { get; set; }               
        public Stream? image_data { get; set; }                
        public required PosterType category { get; set; }    
        public int quantity_available { get; set; } = 0;     
        public string? end_date { get; set; }                

        public DateTime? GetEndDateTime()
        {
            if (DateTime.TryParseExact(end_date, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }
            return null;
        }
    }
}
