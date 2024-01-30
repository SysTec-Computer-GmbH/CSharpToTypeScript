using System;
using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.CLITool.Commands;
using CSharpToTypeScript.CLITool.Utilities;

namespace CSharpToTypeScript.CLITool.Validation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OutputMatchesInput : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            return ValidationResult.Success;
        }
    }
}