namespace Profiles.Application.Common.Constants;

public static class UserRoles
{
    public const string Doctor = "Doctor";
    public const string Receptionist = "Receptionist";
    public const string Patient = "Patient";
    public const string Admin = "Admin";

    public const string ReceptionistOrAdmin = "Receptionist,Admin";
    public const string DoctorOrReceptionistOrAdmin = "Doctor,Receptionist,Admin";
    public const string PatientOrReceptionistOrAdmin = "Patient,Receptionist,Admin";
    public const string Staff = "Doctor,Receptionist,Admin";
    public const string All = "Patient,Doctor,Receptionist,Admin";
}
