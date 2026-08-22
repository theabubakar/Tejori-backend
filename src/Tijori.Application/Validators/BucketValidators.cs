using FluentValidation;
using Tijori.Application.Common;

namespace Tijori.Application.Validators;

public class AddCustomCategoryRequestValidator : AbstractValidator<AddCustomCategoryRequest>
{
    public AddCustomCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}

public class AddCategoryFormFieldRequestValidator : AbstractValidator<AddCategoryFormFieldRequest>
{
    public AddCategoryFormFieldRequestValidator()
    {
        RuleFor(x => x.Label)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.FieldKey)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.FieldType)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.OptionsJson)
            .MaximumLength(2000);
    }
}

public class CreateBucketRequestValidator : AbstractValidator<CreateBucketRequest>
{
    public CreateBucketRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.Remarks)
            .MaximumLength(2000);

        When(x => x.Contract is not null, () =>
        {
            RuleFor(x => x.Contract!.ContractName)
                .NotEmpty()
                .MaximumLength(200);
        });

        When(x => x.Warranty is not null, () =>
        {
            RuleFor(x => x.Warranty!.BrandName)
                .NotEmpty()
                .MaximumLength(200);
        });
    }
}
