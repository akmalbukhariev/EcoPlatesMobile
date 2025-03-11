using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Services
{
    public class CompanyApiService : ApiService
    {
        private const string LOGIN_COMPANY = "login";
        private const string LOGOUT_COMPANY = "logout";
        private const string REGISTER_COMPANY = "registerCompany";
        private const string GET_COMPANY = "getCompany";
        private const string UPDATE_COMPANY_INFO = "updateUserInfo";
        private const string REGISTER_POSTER = "registerPoster";
        private const string GET_POSTER = "getPosterByCompanyID";
        private const string DELETE_POSTER = "deletePoster";

        public CompanyApiService(RestClient client) : base(client)
        {
        }
    }
}
