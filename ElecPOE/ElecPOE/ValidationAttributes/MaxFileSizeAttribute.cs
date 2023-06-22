using System.ComponentModel.DataAnnotations;

namespace ElecPOE.ValidationAttributes
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {

        private readonly int _maxFileSize;

        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            
            if (file != null) 
            {
                if(file.Length > (1000000 * _maxFileSize))
                {
                    return new ValidationResult("File Size is too large");
                }
            }

            return base.IsValid(value, validationContext);
        }
    }
}
