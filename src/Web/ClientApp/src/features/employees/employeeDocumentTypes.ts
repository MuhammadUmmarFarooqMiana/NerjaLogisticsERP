// Mirrors Domain/Enums/EmployeeDocumentType.cs — value is the raw int the
// backend expects, enumName is the exact C# member name the DTO's
// `type` field returns via .ToString(). `required: true` mirrors the
// RequiredDocumentTypes list enforced server-side in
// SubmitProfileForReviewCommandHandler — kept in sync manually since the
// two live in different layers/languages.
export const EMPLOYEE_DOCUMENT_TYPES = [
  { value: 0, key: 'profilePicture', enumName: 'ProfilePicture', required: true },
  { value: 1, key: 'iqama', enumName: 'Iqama', required: false },
  { value: 2, key: 'drivingLicense', enumName: 'DrivingLicense', required: false },
  { value: 3, key: 'passport', enumName: 'Passport', required: false },
  { value: 4, key: 'bikeRegistration', enumName: 'BikeRegistration', required: false },
  { value: 5, key: 'insurance', enumName: 'Insurance', required: false },
  { value: 6, key: 'employmentContract', enumName: 'EmploymentContract', required: false },
  { value: 7, key: 'platformIdProof', enumName: 'PlatformIdProof', required: true },
] as const;

export const REQUIRED_EMPLOYEE_DOCUMENT_TYPES = EMPLOYEE_DOCUMENT_TYPES.filter((t) => t.required);
