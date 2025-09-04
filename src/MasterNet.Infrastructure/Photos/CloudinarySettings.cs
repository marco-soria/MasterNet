namespace MasterNet.Infrastructure.Photos;

/// <summary>
/// Configuración para la integración con Cloudinary.
/// Estas propiedades se cargan desde appsettings.json y son REQUERIDAS.
/// </summary>
public class CloudinarySettings
{
    public string CloudName { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
    public string ApiSecret { get; set; } = default!;
}