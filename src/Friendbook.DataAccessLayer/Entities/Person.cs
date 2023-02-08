﻿using System.Diagnostics.CodeAnalysis;

namespace Friendbook.DataAccessLayer.Entities;

[ExcludeFromCodeCoverage]
public class Person
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string City { get; set; }

    public string SecurityCode { get; set; }

    public byte[] Photo { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; }
}
