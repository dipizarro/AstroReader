using System;

namespace AstroReader.Application.PersonalProfiles.Exceptions;

public class PersonalProfileIntegrityException : Exception
{
    public PersonalProfileIntegrityException(string message)
        : base(message)
    {
    }
}
