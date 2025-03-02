﻿namespace CQRSPattern.Infrastructure.Persistence.Entities;

public class EmployeeEntity
{
    /// <summary>
    /// Emp id.
    /// </summary>
    public int Id { get; set; }
 
    /// <summary>
    /// Birth date.
    /// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// First name.
    /// </summary>
    public string FirtName { get; set; }

    /// <summary>
    /// Last name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Gender.
    /// </summary>
    public string Gender { get; set; }

    /// <summary>
    /// Hire date.
    /// </summary>
    public DateTime HireDate { get; set; }
}