using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ModelValidation
{
    public class CheckFileExtension:ValidationAttribute
    {
        private readonly string[] _extension;
        public CheckFileExtension(string[] exstension)
        {
            _extension = exstension;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extension.Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }
        public string GetErrorMessage()
        {
            return $"This image file extension is not allowed!";
        }
    }
}
