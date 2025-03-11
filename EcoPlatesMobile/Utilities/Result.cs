using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Utilities
{
    public enum Result
    {
        SUCCESS = 100,
        FAILED = 101,

        USER_EXIST = 140,
        USER_NOT_EXIST = 141,
        COMPANY_NOT_EXIST = 142,
        COMPANY_EXIST = 143,
        BRANCH_NOT_EXIST = 144,
        BRANCH_EXIST = 145,
        POSTER_EXIST = 146,
        POSTER_NOT_EXIST = 147,
        POSTER_COMMENT_NOT_EXIST = 148,
        POSTER_COMMENT_EXIST = 149,
        POSTER_FEEDBACK_EXIST = 150,
        POSTER_FEEDBACK_NOT_EXIST = 151,
        PROMOTION_EXIST = 152,
        PROMOTION_NOT_EXIST = 153,
        USER_PASSWORD_NOT_MATCHED = 154,
        NOT_FOUND = 155,
        FOUND = 156,

        TOKEN_INVALID = 200,
        TOKEN_EXPIRED_TIME = 201,
        TOKEN_UNSUPPORTED_JWT = 202,
        LOGIN_INVALID_TOKEN = 250,
        LOGIN_DUPLICATE = 251,
        LOGIN = 252,
        LOGIN_INACTIVE = 253,
        LOGIN_BANNED = 254,
        PASSWORD_IS_NOT_MATCHED = 255,

        VERIFY_PHONE_NUMBER_FAILED = 256,

        AUTHENTICATION_ERROR = 300,
        INTERNAL_ERROR = 301,
        SERVER_ERROR = 302,
        TOKEN_EMPTY = 360,

        API_SERVICE_ERROR = 361,
        JSON_PARSING_ERROR = 362,
        UNKNOWN_ERROR = 363
    }
     
    public static class ResultExtensions
    {
        private static readonly Dictionary<Result, string> _messages = new()
    {
        { Result.SUCCESS, "Success." },
        { Result.FAILED, "Failed." },
        { Result.USER_EXIST, "User exist" },
        { Result.USER_NOT_EXIST, "User is not exist" },
        { Result.COMPANY_NOT_EXIST, "Company is not exist" },
        { Result.COMPANY_EXIST, "Company is exist" },
        { Result.BRANCH_NOT_EXIST, "Branch is not exist" },
        { Result.BRANCH_EXIST, "Branch is exist" },
        { Result.POSTER_EXIST, "Poster is exist" },
        { Result.POSTER_NOT_EXIST, "Poster is not exist" },
        { Result.POSTER_COMMENT_NOT_EXIST, "Poster comment is not exist" },
        { Result.POSTER_COMMENT_EXIST, "Poster comment is exist" },
        { Result.POSTER_FEEDBACK_EXIST, "Poster feedback is exist" },
        { Result.POSTER_FEEDBACK_NOT_EXIST, "Poster feedback is not exist" },
        { Result.PROMOTION_EXIST, "Promotion is exist" },
        { Result.PROMOTION_NOT_EXIST, "Promotion is not exist" },
        { Result.USER_PASSWORD_NOT_MATCHED, "Password is not matched!" },
        { Result.NOT_FOUND, "Not found!" },
        { Result.FOUND, "Found!" },
        { Result.TOKEN_INVALID, "Invalid token information." },
        { Result.TOKEN_EXPIRED_TIME, "This token is expired." },
        { Result.TOKEN_UNSUPPORTED_JWT, "Unsupported token information." },
        { Result.LOGIN_INVALID_TOKEN, "Token information cannot be verified." },
        { Result.LOGIN_DUPLICATE, "Duplicate login." },
        { Result.LOGIN, "Please log in first." },
        { Result.LOGIN_INACTIVE, "Please log in first." },
        { Result.LOGIN_BANNED, "User is banned. Access denied." },
        { Result.PASSWORD_IS_NOT_MATCHED, "Password is not matched" },
        { Result.VERIFY_PHONE_NUMBER_FAILED, "The phone verification failed." },
        { Result.AUTHENTICATION_ERROR, "Your authentication information cannot be verified." },
        { Result.INTERNAL_ERROR, "Something went wrong on our end. We're working to fix it." },
        { Result.SERVER_ERROR, "A system error has occurred. Please contact your administrator." },
        { Result.TOKEN_EMPTY, "Empty token" },
        { Result.API_SERVICE_ERROR, "Empty or invalid response from server" },
        { Result.JSON_PARSING_ERROR, "JSON Parsing Error" },
        { Result.UNKNOWN_ERROR, "Unknown error occurred" }
    };

        public static string GetMessage(this Result result)
        {
            return _messages.TryGetValue(result, out var message) ? message : "Unknown result code.";
        }

        public static string GetCodeToString(this Result result)
        {
            return ((int)result).ToString();
        }

        public static Result? GetResultByCode(int code)
        {
            if (Enum.IsDefined(typeof(Result), code))
            {
                return (Result)code;
            }
            return null;
        }
    }
}
