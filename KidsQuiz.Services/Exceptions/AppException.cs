using System;
using System.Collections.Generic;

namespace KidsQuiz.Services.Exceptions
{
    public class AppException : Exception
    {
        public string ErrorCode { get; }
        public Dictionary<string, string[]> ValidationErrors { get; }

        public AppException(string message, string errorCode = null, Dictionary<string, string[]> validationErrors = null)
            : base(message)
        {
            ErrorCode = errorCode;
            ValidationErrors = validationErrors;
        }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string entityName, object id)
            : base($"{entityName} with id {id} was not found.", "NOT_FOUND")
        {
        }
    }

    public class ValidationException : AppException
    {
        public ValidationException(string message, Dictionary<string, string[]> validationErrors)
            : base(message, "VALIDATION_ERROR", validationErrors)
        {
        }
    }

    public class BusinessRuleException : AppException
    {
        public BusinessRuleException(string message)
            : base(message, "BUSINESS_RULE_VIOLATION")
        {
        }
    }

    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message)
            : base(message, "UNAUTHORIZED")
        {
        }
    }

    public class ConflictException : AppException
    {
        public ConflictException(string message)
            : base(message, "CONFLICT")
        {
        }
    }
} 