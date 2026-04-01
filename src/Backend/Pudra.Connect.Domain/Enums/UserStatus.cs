namespace Pudra.Connect.Domain.Enums;

public enum UserStatus
{
    Pending = 0,    // Onay bekliyor
    Approved = 1,   // Onaylandı, giriş yapabilir
    Rejected = 2    // Reddedildi
}