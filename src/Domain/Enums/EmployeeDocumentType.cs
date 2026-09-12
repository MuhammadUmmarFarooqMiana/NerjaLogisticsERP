namespace NerjaLogisticsERP.Domain.Enums;

// ProfilePicture used to be a value here, uploaded through the same generic document
// endpoint as everything else — but nothing ever read it back from EmployeeDocuments, so
// a rider could satisfy the "required" check during onboarding without their photo ever
// becoming their actual displayed avatar (that only ever came from Employee.
// ProfilePictureStorageKey, set exclusively by UploadMyProfilePictureCommand). Removed in
// favor of that dedicated field being the one and only place a profile picture lives — see
// the AddEmployeeProfilePicture and RemoveEmployeeDocumentProfilePictureType migrations.
public enum EmployeeDocumentType
{
    Iqama,
    DrivingLicense,
    Passport,
    BikeRegistration,
    Insurance,
    EmploymentContract,
    PlatformIdProof
}
