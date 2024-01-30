using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using CSharpToTypeScript.CLITool.Commands;
using CSharpToTypeScript.CLITool.Utilities;

namespace CSharpToTypeScript.CLITool.Validation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InputExists : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return ValidationResult.Success;
        }
    }
}