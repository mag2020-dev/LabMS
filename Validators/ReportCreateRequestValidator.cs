global using Microsoft.IdentityModel.Tokens;
global using System.IdentityModel.Tokens.Jwt;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using System.Security.Claims;
global using System.Text;
global using System.Security.Cryptography;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using System.ComponentModel.DataAnnotations;
global using Mapster;
global using MapsterMapper;
global using LabMS.Entities;
global using LabMS.Data;
global using LabMS.Contracts;
global using LabMS.Services;
global using LabMS.Contracts.Authentication;
global using LabMS.Contracts.Doctor;
global using LabMS.Contracts.Invoice;
global using LabMS.Contracts.LabTest;
global using LabMS.Contracts.Patient;
global using LabMS.Contracts.Payment;
global using LabMS.Contracts.Report;
global using LabMS.Contracts.Role;
global using LabMS.Contracts.TestOrder;
global using LabMS.Contracts.TestResult;
global using LabMS.Contracts.User;
global using LabMS.Contracts.Visit;

namespace LabMS.Validators;

public class ReportCreateRequestValidator : AbstractValidator<ReportCreateRequest>
{
    public ReportCreateRequestValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty().WithMessage("Visit ID is required");

        RuleFor(x => x.FilePath)
            .NotEmpty().WithMessage("File path is required")
            .MaximumLength(500).WithMessage("File path cannot exceed 500 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_./\\]+$").WithMessage("File path contains invalid characters");
    }
}
