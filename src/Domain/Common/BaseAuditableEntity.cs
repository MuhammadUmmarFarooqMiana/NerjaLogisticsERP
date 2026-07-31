namespace NerjaLogisticsERP.Domain.Common;

//A practical guideline

//As we're building ERP, use this rule:

//Simple data with no business rules → a setter(or a straightforward update method) is fine.
//Anything that changes multiple fields, enforces validation, or represents a business action → make it a method on the entity.

//That keeps your domain model expressive without becoming overly complicated.

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTimeOffset Created { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTimeOffset? LastModified { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public bool IsDeleted { get; private set; }

    public DateTimeOffset? DeletedAt { get; private set; }

    public Guid? DeletedBy { get; private set; }

    public void Delete(Guid? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
        DeletedBy = deletedBy;
    }


    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }

}
