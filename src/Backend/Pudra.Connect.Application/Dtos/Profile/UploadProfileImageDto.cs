namespace Pudra.Connect.Application.Dtos.Profile;

public class UploadProfileImageDto
{
    public string ImageBase64 { get; set; } = string.Empty; // "data:image/png;base64,..." formatında
}