using System;
using System.ComponentModel.DataAnnotations;

namespace GymClass.Validations
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public int MinutesOffset { get; set; } = 0;

        public FutureDateAttribute() : base("A data deve ser futura")
        {
        }

        public FutureDateAttribute(int minutesOffset) : base($"A data deve ser pelo menos {minutesOffset} minutos no futuro")
        {
            MinutesOffset = minutesOffset;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value is not DateTime dateValue)
            {
                return new ValidationResult("Valor não é uma data válida");
            }

            if (value is DateTime date && date <= DateTime.UtcNow)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            if (MinutesOffset > 0)
            {
                return $"{name} deve ser pelo menos {MinutesOffset} minutos no futuro";
            }

            return $"{name} deve ser uma data futura";
        }
    }
}
