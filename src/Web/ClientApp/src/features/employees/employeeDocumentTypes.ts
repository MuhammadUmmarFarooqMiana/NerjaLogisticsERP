// Mirrors Domain/Enums/EmployeeDocumentType.cs — value is the raw int the backend
// expects on upload (UploadEmployeeDocumentCommand.Type binds this enum by numeric
// ordinal; no JsonStringEnumConverter is configured, same convention as
// vehicleTypes.ts/salaryFormulaEnums.ts/leaveStatus.ts/expenseCategories.ts), enumName
// is the exact C# member name the DTO's `type` field returns via .ToString() (that
// read path IS string-based, so it's immune to ordinal shifts). `required: true`
// mirrors the RequiredDocumentTypes list enforced server-side in
// SubmitProfileForReviewCommandHandler — kept in sync manually since the two live in
// different layers/languages.
//
// IMPORTANT: `value` must match the C# enum's ordinal position exactly. Iqama starts
// at 0 because ProfilePicture (formerly ordinal 0) was removed — see the comment below
// and RemoveEmployeeDocumentProfilePictureType's migration comment for why. Adding,
// removing, or reordering a member in EmployeeDocumentType.cs requires renumbering
// every value here to match.
//
// The profile picture is NOT in this list — it isn't an EmployeeDocument at all, it's
// Employee.ProfilePictureStorageKey, uploaded through UploadMyProfilePictureCommand and
// rendered by ProfilePictureUploader/UserAvatar instead. It used to be listed here, but
// nothing ever read that upload back out, so it satisfied this "required" check without
// ever becoming the rider's actual displayed avatar — see
// RemoveEmployeeDocumentProfilePictureType's migration comment for the full story.
export const EMPLOYEE_DOCUMENT_TYPES = [
  { value: 0, key: 'iqama', enumName: 'Iqama', required: false },
  { value: 1, key: 'drivingLicense', enumName: 'DrivingLicense', required: false },
  { value: 2, key: 'passport', enumName: 'Passport', required: false },
  { value: 3, key: 'bikeRegistration', enumName: 'BikeRegistration', required: false },
  { value: 4, key: 'insurance', enumName: 'Insurance', required: false },
  { value: 5, key: 'employmentContract', enumName: 'EmploymentContract', required: false },
  { value: 6, key: 'platformIdProof', enumName: 'PlatformIdProof', required: true },
] as const;

export const REQUIRED_EMPLOYEE_DOCUMENT_TYPES = EMPLOYEE_DOCUMENT_TYPES.filter((t) => t.required);
