using System.ComponentModel.DataAnnotations;
using ValidationException = Profiles.Application.Common.Exceptions.ValidationException;

namespace Profiles.Application.Common.Helpers;

public static class PersonValidationHelper
{
    public static void ValidateName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ValidationException("First name is required.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ValidationException("Last name is required.");
        }
    }

    public static void ValidateDateOfBirth(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (dateOfBirth > today)
        {
            throw new ValidationException("Date of birth cannot be later than current date.");
        }
    }

    public static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ValidationException("Please, enter the email.");
        }

        if (!new EmailAddressAttribute().IsValid(email))
        {
            throw new ValidationException("You've entered an invalid email.");
        }
    }

    public static void ValidateOfficeId(Guid officeId)
    {
        if (officeId == Guid.Empty)
        {
            throw new ValidationException("Office is required.");
        }
    }
}
